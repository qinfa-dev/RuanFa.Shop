using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Domain.Accounts.Entities;
using RuanFa.FashionShop.Domain.Todos.AggregateRoot;
using RuanFa.FashionShop.Domain.Todos.Entities;

namespace RuanFa.FashionShop.Application.Abstractions.Data;
public interface IApplicationDbContext
{
    DbSet<UserProfile> Profiles { get; }
    DbSet<TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}
