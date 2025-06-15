using Carter;
using RuanFa.FashionShop.Web.Api.Extensions;
using RuanFa.FashionShop.Web.Api.Middlewares;
using Scalar.AspNetCore;
using Serilog;
using System.Text.Json.Serialization;
using System.Text.Json;
using Coravel;
using Microsoft.Extensions.Options;
using RuanFa.FashionShop.Infrastructure.BackgroundJobs;
using RuanFa.FashionShop.Infrastructure.Settings;
using Ardalis.GuardClauses;

namespace RuanFa.FashionShop.Web.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        // Configure System.Text.Json for controllers
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                ConfigureJsonOptions(options.JsonSerializerOptions);
            });

        // Configure System.Text.Json for HTTP (used by TypedResults)
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            ConfigureJsonOptions(options.SerializerOptions);
        });

        // Carter with System.Text.Json response negotiator
        services.AddCarter();
        services.AddOpenApi(options => options
            .AddDocumentTransformer<BearerSecuritySchemeTransformer>());
        //services.AddCors(options => options.AddPolicy("AllowAngularDevServer",
        //        policy => policy.WithOrigins("http://localhost:4200")
        //        .AllowAnyHeader()
        //        .AllowAnyMethod()));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerWithAuth();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails(ConfigureProblemDetails);

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromDays(30); // Session expiry
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
        services.AddScheduler();
        return services;
    }
    public static IApplicationBuilder UsePresentation(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerWithUi();
            app.MapScalarApiReference(options =>
            {
                options.WithOpenApiRoutePattern("/openapi/v1.json");
                options.Theme = ScalarTheme.None;
                options.AddPreferredSecuritySchemes("Bearer");
            });

        }
        app.UseRouting();
        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseExceptionHandler();

        app.UseSerilogRequestLogging();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        //app.UseCors("AllowAngularDevServer");

        app.MapControllers();
        app.MapCarter();

        return app;
    }

    private static void ConfigureProblemDetails(ProblemDetailsOptions options)
    {
        options.CustomizeProblemDetails = context =>
        {
            var traceId = context.HttpContext.TraceIdentifier;
            var userAgent = context.HttpContext.Request.Headers.UserAgent.ToString();

            context.ProblemDetails.Extensions["traceId"] = traceId;
            context.ProblemDetails.Extensions["userAgent"] = userAgent;
        };
    }
    private static void ConfigureJsonOptions(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.Converters.Add(new SystemTextJsonUtcDateTimeOffsetConverter());
        options.Converters.Add(new SystemTextJsonNullableDateTimeOffsetConverter());
    }

    public static async Task ScheduleBackgroundJobsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var jobSettings = scope.ServiceProvider
            .GetRequiredService<IOptions<BackgroundJobsSettings>>().Value;

        Guard.Against.Null(jobSettings, message: "BackgroundJobsSettings not found in configuration.");

        if (jobSettings.Cleanup.Enabled)
        {
            // Build a UTC DateTime for the scheduled time
            var utcTime = new DateTime(
                1, 1, 1,
                jobSettings.Cleanup.ScheduleHour,
                jobSettings.Cleanup.ScheduleMinute,
                0,
                DateTimeKind.Utc);

            // Convert to local time (Coravel schedules based on local system time)
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);

            app.Services.UseScheduler(scheduler =>
            {
                if (localTime.Hour == 0 && localTime.Minute == 0)
                    scheduler.Schedule<DeleteOldActivityLogsJob>()
                             .Daily();
                else
                    scheduler.Schedule<DeleteOldActivityLogsJob>()
                             .DailyAt(localTime.Hour, localTime.Minute);

                //scheduler.Schedule<TodoReminderJob>()
                //    .EveryMinute();
            });

            Log.Information(
                "✅ Cleanup job scheduled at {UtcHour:D2}:{UtcMinute:D2} (24h) UTC / {LocalHour:D2}:{LocalMinute:D2} (24h) Local",
                jobSettings.Cleanup.ScheduleHour,
                jobSettings.Cleanup.ScheduleMinute,
                localTime.Hour,
                localTime.Minute);
        }

        await Task.CompletedTask; 
    }

}
