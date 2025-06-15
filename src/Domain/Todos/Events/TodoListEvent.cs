using RuanFa.FashionShop.Domain.Todos.AggregateRoot;
using RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

namespace RuanFa.FashionShop.Domain.Todos.Events;
public sealed record TodoListCreatedEvent(TodoList TodoList) : IDomainEvent;
public sealed record TodoListDeletedEvent(TodoList TodoList) : IDomainEvent;
public sealed record TodoListUpdatedEvent(TodoList TodoList) : IDomainEvent;
