using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Contexts;
using RuanFa.FashionShop.SharedKernel.Interfaces.Domain;
using RuanFa.FashionShop.SharedKernel.Interfaces.System;

namespace RuanFa.FashionShop.Infrastructure.Data.Interceptors;

internal sealed class AuditableEntityInterceptor(IUserContext userContext, IDateTimeProvider dateTimeProvider)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null || !userContext.IsAuthenticated)
            return;

        var now = dateTimeProvider.UtcNow;
        var userString = userContext.UserId != null ? "System" : userContext.Username;

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userString;
                entry.Entity.LastModified = now;
                entry.Entity.LastModifiedBy = userString;
            }
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                // Prevent updating Created fields
                entry.Property(nameof(IAuditable.CreatedAt)).IsModified = false;
                entry.Property(nameof(IAuditable.CreatedBy)).IsModified = false;

                entry.Entity.LastModified = now;
                entry.Entity.LastModifiedBy = userString;
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
