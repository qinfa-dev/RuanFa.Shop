using Carter;
using Mapster;
using MediatR;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Attributes;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;
using RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Create;
using RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Delete;
using RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Get;
using RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Update;
using RuanFa.FashionShop.Application.Accounts.Models.Datas;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Web.Api.Extensions;

namespace RuanFa.FashionShop.Web.Api.Accounts;

public class RoleMoudle : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles")
            .WithTags("Roles")
            .WithOpenApi();

        // Create Role
        group.MapPost("/",
            [ApiAuthorize(Permission.Role.Create)] async (RoleInfo request, ISender mediator) =>
            {
                var command = request.Adapt<CreateRoleCommand>();
                var result = await mediator.Send(command);
                return result.ToTypedResultCreated($"/roles/{result.Value?.Id}");
            })
            .WithName("CreateRole")
            .WithDescription("Creates a new role and returns the created role's profile.")
            .WithSummary("Create a new role")
            .Produces<RoleResult>()
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Get Role by ID
        group.MapGet("/{roleId}",
            [ApiAuthorize(Permission.Role.Get)] async (Guid roleId, ISender mediator) =>
            {
                var query = new GetRoleByIdQuery(roleId);
                var result = await mediator.Send(query);
                return result.ToTypedResult();
            })
            .WithName("GetRoleById")
            .WithDescription("Retrieves the role details by their unique ID.")
            .WithSummary("Get role by ID")
            .Produces<RoleDetailResult>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Get Role List
        group.MapGet("/",
            [ApiAuthorize(Permission.Role.Get)] async (ISender mediator) =>
            {
                var query = new GetRolesQuery();
                var result = await mediator.Send(query);
                return result.ToTypedResult();
            })
            .WithName("GetRoleList")
            .WithDescription("Retrieves a list of roles with filtering, sorting, and pagination options.")
            .WithSummary("Get list of roles")
            .Produces<List<RoleResult>>()
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Update Role
        group.MapPut("/{roleId}",
            [ApiAuthorize(Permission.Role.Update)] async (Guid roleId, RoleInfo request, ISender mediator) =>
            {
                var command = request.Adapt<UpdateRoleCommand>();
                command.RoleId = roleId;
                var result = await mediator.Send(command);
                return result.ToTypedResult();
            })
            .WithName("UpdateRole")
            .WithDescription("Updates the details of an existing role by their unique ID.")
            .WithSummary("Update an existing role")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Delete Role
        group.MapDelete("/{roleId}",
            [ApiAuthorize(Permission.Role.Delete)] async (Guid roleId, ISender mediator) =>
            {
                var command = new DeleteRoleCommand(roleId);
                var result = await mediator.Send(command);
                return result.ToTypedResultDeleted();
            })
            .WithName("DeleteRole")
            .WithDescription("Deletes an existing role by their unique ID.")
            .WithSummary("Delete a role")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }
}
