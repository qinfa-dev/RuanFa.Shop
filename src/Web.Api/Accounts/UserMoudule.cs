using Carter;
using Mapster;
using MediatR;
using RuanFa.FashionShop.Application.Abstractions.Models.Options;
using RuanFa.FashionShop.Application.Abstractions.Models.Pagings;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Attributes;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;
using RuanFa.FashionShop.Application.Accounts.Models.Datas;
using RuanFa.FashionShop.Application.Accounts.Models.Requests;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Users.Assign;
using RuanFa.FashionShop.Application.Accounts.Users.Create;
using RuanFa.FashionShop.Application.Accounts.Users.Delete;
using RuanFa.FashionShop.Application.Accounts.Users.Get;
using RuanFa.FashionShop.Application.Accounts.Users.List;
using RuanFa.FashionShop.Application.Accounts.Users.Update;
using RuanFa.FashionShop.Web.Api.Extensions;

namespace RuanFa.FashionShop.Web.Api.Accounts;

public class UsersModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .WithOpenApi();

        // Create User
        group.MapPost("/",
            [ApiAuthorize(Permission.User.Create)] async (UserAccountInfo request, ISender mediator) =>
            {
                var command = request.Adapt<CreateAccountCommand>();
                var result = await mediator.Send(command);
                return result.ToTypedResultCreated($"/users/{result.Value?.Id}");
            })
            .WithName("CreateUser")
            .WithDescription("Creates a new user and returns the created user's profile.")
            .WithSummary("Create a new user")
            .Produces<AccountProfieResult>()
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Get User by ID
        group.MapGet("/{userId}",
            [ApiAuthorize(Permission.User.Get)] async (Guid userId, ISender mediator) =>
            {
                var query = new GetAccountProfileQuery(userId);
                var result = await mediator.Send(query);
                return result.ToTypedResult();
            })
            .WithName("GetUserById")
            .WithDescription("Retrieves the user details by their unique ID.")
            .WithSummary("Get user by ID")
            .Produces<AccountProfieResult>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Get User List
        group.MapGet("/",
            [ApiAuthorize(Permission.User.Get)] async (
                [AsParameters] SearchOptions search,
                [AsParameters] PaginationOption pagination,
                [AsParameters] SortOption sort,
                ISender mediator) =>
            {
                var query = new GetAccountProfiesQuery
                {
                    Search = search,
                    Pagination = pagination,
                    Sort = sort
                };
                var result = await mediator.Send(query);
                return result.ToTypedResult();
            })
            .WithName("GetUserList")
            .WithDescription("Retrieves a list of users with filtering, sorting, and pagination options.")
            .WithSummary("Get list of users")
            .Produces<PagedList<AccountProfieResult>>()
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Update User
        group.MapPut("/{userId}",
            [ApiAuthorize(Permission.Role.Update)] async (Guid userId, UserProfileInfo request, ISender mediator) =>
            {
                var command = request.Adapt<UpdateAccountCommand>();
                command.UserId = userId;
                var result = await mediator.Send(command);
                return result.ToTypedResult();
            })
            .WithName("UpdateUser")
            .WithDescription("Updates the details of an existing user by their unique ID.")
            .WithSummary("Update an existing user")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Delete User
        group.MapDelete("/{userId}",
            [ApiAuthorize(Permission.User.Delete)] async (Guid userId, ISender mediator) =>
            {
                var command = new DeleteAccountCommand(userId);
                var result = await mediator.Send(command);
                return result.ToTypedResultDeleted();
            })
            .WithName("DeleteUser")
            .WithDescription("Deletes an existing user by their unique ID.")
            .WithSummary("Delete a user")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Assign Roles to User
        group.MapPost("/{userId}/roles",
            [ApiAuthorize(Permission.Role.Users)] async (Guid userId, List<Guid> roles, ISender mediator) =>
            {
                var command = new AssignUserRolesCommand(userId, roles);
                var result = await mediator.Send(command);
                return result.ToTypedResult();
            })
            .WithName("AssignRolesToUser")
            .WithDescription("Assigns one or more roles to a user by their unique ID.")
            .WithSummary("Assign roles to a user")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }
}
