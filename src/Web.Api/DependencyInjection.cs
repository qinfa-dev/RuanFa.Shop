﻿using Carter;
using RuanFa.FashionShop.Web.Api.Extensions;
using RuanFa.FashionShop.Web.Api.Middlewares;
using Scalar.AspNetCore;
using Serilog;

namespace RuanFa.FashionShop.Web.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
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
}
