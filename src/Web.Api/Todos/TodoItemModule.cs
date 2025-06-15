using Carter;
using Mapster;
using MediatR;
using RuanFa.FashionShop.Application.Abstractions.Models.Options;
using RuanFa.FashionShop.Application.Abstractions.Models.Pagings;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Attributes;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;
using RuanFa.FashionShop.Application.Todos.Items.Complete;
using RuanFa.FashionShop.Application.Todos.Items.Create;
using RuanFa.FashionShop.Application.Todos.Items.Delete;
using RuanFa.FashionShop.Application.Todos.Items.Get;
using RuanFa.FashionShop.Application.Todos.Items.GetById;
using RuanFa.FashionShop.Application.Todos.Items.Update;
using RuanFa.FashionShop.Application.Todos.Models.Requests;
using RuanFa.FashionShop.Application.Todos.Models.Responses.Items;
using RuanFa.FashionShop.Web.Api.Extensions;

namespace RuanFa.FashionShop.Web.Api.Todos;

public class TodoItemModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todo-items")
            .WithTags("TodoItems")
            .WithOpenApi();

        // Create Todo Item
        group.MapPost("/",
            [ApiAuthorize(Permission.TodoItem.Create)] async (TodoItemInfo request, ISender mediator) =>
            {
                var command = request.Adapt<CreateTodoItemCommand>();
                var result = await mediator.Send(command);
                return result.ToTypedResultCreated($"/todo-items/{result.Value?.Id}");
            })
            .WithName("CreateTodoItem")
            .WithDescription("Creates a new todo item and returns the created item.")
            .WithSummary("Create a new todo item")
            .Produces<TodoItemResult>()
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Get Todo Item by ID
        group.MapGet("/{itemId}",
            [ApiAuthorize(Permission.TodoItem.Get)] async (int itemId, ISender mediator) =>
            {
                var query = new GetTodoItemByIdQuery(itemId);
                var result = await mediator.Send(query);
                return result.ToTypedResult();
            })
            .WithName("GetTodoItemById")
            .WithDescription("Retrieves the todo item details by its unique ID.")
            .WithSummary("Get todo item by ID")
            .Produces<TodoItemResult>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Get Todo Item List
        group.MapGet("/",
            [ApiAuthorize(Permission.TodoItem.Get)] async (
                [AsParameters] SearchOptions search,
                [AsParameters] PaginationOption pagination,
                [AsParameters] SortOption sort,
                ISender mediator) =>
            {
                var query = new GetTodoItemQuery
                {
                    Search = search,
                    Pagination = pagination,
                    Sort = sort
                };
                var result = await mediator.Send(query);
                return result.ToTypedResult();
            })
            .WithName("GetTodoItemList")
            .WithDescription("Retrieves a list of todo items with filtering, sorting, and pagination options.")
            .WithSummary("Get list of todo items")
            .Produces<PagedList<TodoItemResult>>()
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Update Todo Item
        group.MapPut("/{itemId}",
            [ApiAuthorize(Permission.TodoItem.Update)] async (int itemId, TodoItemInfo request, ISender mediator) =>
            {
                var command = request.Adapt<UpdateTodoItemCommand>();
                command.ItemId = itemId;
                var result = await mediator.Send(command);
                return result.ToTypedResult();
            })
            .WithName("UpdateTodoItem")
            .WithDescription("Updates the details of an existing todo item by its unique ID.")
            .WithSummary("Update an existing todo item")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Complete Todo Item
        group.MapPut("/{itemId}/complete",
            [ApiAuthorize(Permission.TodoItem.Complete)] async (int itemId, ISender mediator) =>
            {
                var command = new CompleteTodoItemCommand(itemId);
                var result = await mediator.Send(command);
                return result.ToTypedResult();
            })
            .WithName("CompleteTodoItem")
            .WithDescription("Marks a todo item as completed by its unique ID.")
            .WithSummary("Mark a todo item as complete")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Delete Todo Item
        group.MapDelete("/{itemId}",
            [ApiAuthorize(Permission.TodoItem.Delete)] async (int itemId, ISender mediator) =>
            {
                var command = new DeleteTodoItemCommand(itemId);
                var result = await mediator.Send(command);
                return result.ToTypedResultDeleted();
            })
            .WithName("DeleteTodoItem")
            .WithDescription("Deletes an existing todo item by its unique ID.")
            .WithSummary("Delete a todo item")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }
}
