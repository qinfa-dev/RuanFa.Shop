using MediatR;
using Microsoft.AspNetCore.Mvc;
using RuanFa.FashionShop.Application.Accounts.Authentication.Login;
using RuanFa.FashionShop.Application.Accounts.Authentication.RefreshToken;
using RuanFa.FashionShop.Application.Accounts.Authentication.Register;
using RuanFa.FashionShop.Application.Accounts.Authentication.SocialLogin;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Web.Api.Extensions;
using Carter;
using RuanFa.FashionShop.Application.Accounts.Credentials.Email.Resend;
using RuanFa.FashionShop.Application.Accounts.Credentials.Email.Update;
using RuanFa.FashionShop.Application.Accounts.Credentials.Password.Forgot;
using RuanFa.FashionShop.Application.Accounts.Credentials.Password.Reset;
using RuanFa.FashionShop.Application.Accounts.Credentials.Password.Update;
using RuanFa.FashionShop.Application.Accounts.Credentials.Email.Confirm;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Contexts;
using RuanFa.FashionShop.Application.Accounts.Users.Get;
using RuanFa.FashionShop.Application.Accounts.Models.Datas;
using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Users.Update;

namespace RuanFa.FashionShop.Web.Api.Accounts;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/account")
            .WithTags("Authentication")
            .WithOpenApi();

        // Register
        group.MapPost("/register", Register)
            .WithName("Register")
            .WithDescription("Creates a new user account")
            .WithSummary("Register new user")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Login
        group.MapPost("/login", Login)
            .WithName("Login")
            .WithDescription("Authenticates a user and returns JWT tokens")
            .WithSummary("User login")
            .Produces<TokenResult>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Refresh token
        group.MapPost("/refresh-token", RefreshToken)
            .WithName("RefreshToken")
            .WithDescription("Refreshes an expired access token using a refresh token")
            .WithSummary("Refresh access token")
            .Produces<TokenResult>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Social Login
        group.MapPost("/social-login", SocialLogin)
            .WithName("SocialLogin")
            .WithDescription("Authenticates a user using a social provider and returns access and refresh tokens.")
            .WithSummary("Authenticate via social login")
            .Produces<TokenResult>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Forgot Password
        group.MapPost("/forgot-password", ForgotPassword)
            .WithName("ForgotPassword")
            .WithDescription("Sends a password reset link to the user's email address.")
            .WithSummary("Send password reset link")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Reset Password
        group.MapPost("/reset-password", ResetPassword)
            .WithName("ResetPassword")
            .WithDescription("Resets the user's password using a token.")
            .WithSummary("Reset password with token")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Update Password (authenticated)
        group.MapPost("/update-password", UpdatePassword)
            .WithName("UpdatePassword")
            .WithDescription("Updates the user's current password.")
            .WithSummary("Update current password")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        // Confirm Email
        group.MapGet("/confirm-email", ConfirmEmail)
            .WithName("ConfirmEmail")
            .WithDescription("Confirms a user's email address using a token.")
            .WithSummary("Confirm email with token")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Resend Confirmation Email
        group.MapPost("/resend-confirmation-email", ResendConfirmationEmail)
            .WithName("ResendConfirmationEmail")
            .WithDescription("Resends the confirmation email to the user.")
            .WithSummary("Resend email verification")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Update Email
        group.MapPost("/update-email", UpdateEmail)
            .WithName("UpdateEmail")
            .WithDescription("Updates the user's email address and sends a confirmation email.")
            .WithSummary("Update email address")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        // Get Profile
        group.MapGet("/profile", GetProfile)
            .WithName("GetProfile")
            .WithDescription("Gets the current user's profile information.")
            .WithSummary("Get user profile")
            .Produces<AccountProfieResult>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        // Update Profile
        group.MapPut("/profile", UpdateProfile)
            .WithName("UpdateProfile")
            .WithDescription("Updates the current user's profile information.")
            .WithSummary("Update user profile")
            .Produces<AccountProfieResult>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        // Get Account Info
        group.MapGet("/info", GetAccountInfo)
            .WithName("GetAccountInfo")
            .WithDescription("Gets the current user's account info.")
            .WithSummary("Get account info")
            .Produces<AccountInfoResult>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();
    }

    // Handlers

    private static async Task<IResult> Register(RegisterUserCommand command, [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> Login(LoginWithPasswordQuery query, [FromServices] ISender sender)
    {
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    private static async Task<IResult> RefreshToken(RefreshTokenCommand command, [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> SocialLogin(SocialLoginQuery command, [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> ForgotPassword(ForgotPasswordCommand command, [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> ResetPassword(ResetPasswordCommand command, [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> UpdatePassword(UpdatePasswordCommand command, [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> ConfirmEmail(
        [AsParameters] ConfirmEmailCommand command, 
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> ResendConfirmationEmail(
        ResendConfirmationEmailCommand command, 
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> UpdateEmail(
        UpdateEmailCommand command, 
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> GetProfile(
        [FromServices] ISender sender,
        [FromServices] IUserContext userContext)
    {
        var userId = GetUserId(userContext);
        if (userId.IsError) return userId.ToTypedResult();

        var result = await sender.Send(new GetAccountProfileQuery(userId.Value));
        return result.ToTypedResult();
    }

    private static async Task<IResult> UpdateProfile(
        UserProfileInfo userAccount,
        [FromServices] IUserContext userContext,
        [FromServices] ISender sender)
    {
        var userId = GetUserId(userContext);
        if (userId.IsError) return userId.ToTypedResult();
        UpdateAccountCommand command = (UpdateAccountCommand)userAccount;
        command.UserId = userId.Value;
        var result = await sender.Send(command);
        return result.ToTypedResult();
    }

    private static async Task<IResult> GetAccountInfo(
        [FromServices] ISender sender, 
        [FromServices] IUserContext userContext, 
        CancellationToken cancellationToken)
    {
        var userId = GetUserId(userContext);
        if (userId.IsError) return userId.ToTypedResult();

        var result = await sender.Send(new GetAccountInfoQuery(userId.Value), cancellationToken);
        return result.ToTypedResult();
    }
    private static ErrorOr<Guid> GetUserId (IUserContext userContext)
    {
        var userId = userContext.UserId;

        // Guard: User is authenticated
        if (!userContext.IsAuthenticated || userId is null)
            return DomainErrors.UserProfile.Permissions.UnauthorizedAccess;

        return userId.Value;
    }
}
