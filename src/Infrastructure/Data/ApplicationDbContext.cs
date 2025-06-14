using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Domain.Accounts.Entities;
using RuanFa.FashionShop.Domain.Todos.AggregateRoot;
using RuanFa.FashionShop.Domain.Todos.Entities;
using RuanFa.FashionShop.Infrastructure.Accounts.Entities;
using RuanFa.FashionShop.Infrastructure.Data.Converters;

namespace RuanFa.FashionShop.Infrastructure.Data;
internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options), IApplicationDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyUtcDateTimeConverter();
    }

    // Profiles
    public DbSet<UserProfile> Profiles => Set<UserProfile>();

    // Todo
    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

}
