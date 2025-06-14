using RuanFa.FashionShop.Domain.Todos.Entities;
using RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

namespace RuanFa.FashionShop.Domain.Todos.Events;

public record TodoItemCompletedEvent(TodoItem Item) : IDomainEvent;
