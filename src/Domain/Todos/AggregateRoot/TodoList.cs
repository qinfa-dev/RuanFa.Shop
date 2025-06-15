using RuanFa.FashionShop.Domain.Todos.Entities;
using RuanFa.FashionShop.Domain.Todos.ValueObjects;
using RuanFa.FashionShop.SharedKernel.Domains.AggregateRoots;

namespace RuanFa.FashionShop.Domain.Todos.AggregateRoot;

public class TodoList : AggregateRoot<int>
{
    #region Properties
    public string Title { get; set; }
    public Colour? Colour { get; set; }
    #endregion

    #region Relationship
    public ICollection<TodoItem> Items { get; set; }
    #endregion

    #region Constructor
    private TodoList(string title, Colour? colour)
    {
        Title = title;
        Colour = colour;
        Items = new List<TodoItem>();
    }
    #endregion

    #region Factory Method
    public static TodoList Create(string title, Colour? colour)
    {
        return new TodoList(title.Trim(), colour);
    }
    #endregion
}
