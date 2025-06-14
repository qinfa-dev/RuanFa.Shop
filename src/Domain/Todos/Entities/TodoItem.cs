﻿using ErrorOr;
using RuanFa.FashionShop.SharedKernel.Domains.Entities;
using RuanFa.FashionShop.Domain.Todos.Enums;
using RuanFa.FashionShop.Domain.Todos.Events;
using RuanFa.FashionShop.Domain.Todos.AggregateRoot;

namespace RuanFa.FashionShop.Domain.Todos.Entities;

public class TodoItem : AuditableEnitity<int>
{
    #region Properties
    public string? Title { get; private set; }
    public string? Note { get; private set; }
    public PriorityLevel Priority { get; private set; }
    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value && !_done)
            {
                AddDomainEvent(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }
    public DateTime? DoneAt { get; set; }
    #endregion

    #region Relationship
    public int ListId { get; private set; }
    public TodoList List { get; private set; } = null!;
    #endregion

    #region Contructor
    private TodoItem()
    {
        Priority = PriorityLevel.None;
    }

    private TodoItem(
        string? title,
        string? note,
        PriorityLevel priority,
        int listId) : this()
    {
        Title = title;
        Note = note;
        Priority = priority;
        ListId = listId;
    }
    #endregion

    #region Methods
    public static TodoItem Create(
        string? title,
        string? note,
        int priority,
        int listId)
    {
        var item = new TodoItem(title, note, (PriorityLevel)priority, listId);
        return item;
    }
    public Updated Update(string? title, string? note)
    {
        Title = title;
        Note = note;
        return Result.Updated;
    }

    public Updated UpdatePriority(PriorityLevel priority)
    {
        Priority = priority;
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateStatus(bool isDone, DateTime doneAt)
    {
        if (CreatedAt > doneAt)
        {
            return DomainErrors.TodoItem.Validation.InvalidDoneAt;
        }
        Done = isDone;
        DoneAt = doneAt;
        return Result.Updated;
    }
    #endregion
}
