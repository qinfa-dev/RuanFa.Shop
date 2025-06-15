using Carter;
using Mapster;
using MediatR;
using RuanFa.FashionShop.Application.Abstractions.Models.Options;
using RuanFa.FashionShop.Application.Abstractions.Models.Pagings;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Attributes;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;
using RuanFa.FashionShop.Application.Todos.Lists.Create;
using RuanFa.FashionShop.Application.Todos.Lists.Delete;
using RuanFa.FashionShop.Application.Todos.Lists.Get;
using RuanFa.FashionShop.Application.Todos.Lists.GetById;
using RuanFa.FashionShop.Application.Todos.Lists.Update;
using RuanFa.FashionShop.Application.Todos.Models.Requests;
using RuanFa.FashionShop.Application.Todos.Models.Responses.Lists;
using RuanFa.FashionShop.Web.Api.Extensions;

namespace RuanFa.FashionShop.Web.Api.Todos;

public class TodoListModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todo-lists")
            .WithTags("TodoLists")
            .WithOpenApi();

        // Create Todo List
        group.MapPost("/",
            [ApiAuthorize(Permission.TodoList.Create)] async (TodoListInfo request, ISender mediator) =>
            {
                var command = request.Adapt<CreateTodoListCommand>();
                var result = await mediator.Send(command);
                return result.ToTypedResultCreated($"/todo-lists/{result.Value?.Id}");
            })
            .WithName("CreateTodoList")
            .WithDescription("Creates a new todo list and returns the created list.")
            .WithSummary("Create a new todo list")
            .Produces<TodoListResult>()
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Get Todo List by ID
        group.MapGet("/{listId}",
            [ApiAuthorize(Permission.TodoList.Get)] async (int listId, ISender mediator) =>
            {
                var query = new GetTodoListByIdQuery(listId);
                var result = await mediator.Send(query);
                return result.ToTypedResult();
            })
            .WithName("GetTodoListById")
            .WithDescription("Retrieves the todo list details by its unique ID.")
            .WithSummary("Get todo list by ID")
            .Produces<TodoListResult>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Get Todo List List
        group.MapGet("/",
            [ApiAuthorize(Permission.TodoList.Get)] async (
                [AsParameters] SearchOptions search,
                [AsParameters] PaginationOption pagination,
                [AsParameters] SortOption sort,
                ISender mediator) =>
            {
                var query = new GetTodoListQuery
                {
                    Search = search,
                    Pagination = pagination,
                    Sort = sort
                };
                var result = await mediator.Send(query);
                return result.ToTypedResult();
            })
            .WithName("GetTodoListList")
            .WithDescription("Retrieves a list of todo lists with filtering, sorting, and pagination options.")
            .WithSummary("Get list of todo lists")
            .Produces<PagedList<TodoListResult>>()
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Update Todo List
        group.MapPut("/{listId}",
            [ApiAuthorize(Permission.TodoList.Update)] async (int listId, TodoListInfo request, ISender mediator) =>
            {
                var command = request.Adapt<UpdateTodoListCommand>();
                command.ListId = listId;
                var result = await mediator.Send(command);
                return result.ToTypedResult();
            })
            .WithName("UpdateTodoList")
            .WithDescription("Updates the details of an existing todo list by its unique ID.")
            .WithSummary("Update an existing todo list")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Delete Todo List
        group.MapDelete("/{listId}",
            [ApiAuthorize(Permission.TodoList.Delete)] async (int listId, ISender mediator) =>
            {
                var command = new DeleteTodoListCommand(listId);
                var result = await mediator.Send(command);
                return result.ToTypedResultDeleted();
            })
            .WithName("DeleteTodoList")
            .WithDescription("Deletes an existing todo list by its unique ID.")
            .WithSummary("Delete a todo list")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }
}
