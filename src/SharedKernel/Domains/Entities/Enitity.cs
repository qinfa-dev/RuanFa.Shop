using RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

namespace RuanFa.FashionShop.SharedKernel.Domains.Entities;

public abstract class Entity<TId> : IHasDomainEvent, IEquatable<Entity<TId>> where TId : notnull
{
    #region Fields
    private readonly List<IDomainEvent> _domainEvents = [];

    #endregion

    #region Properties
    public TId Id { get; protected set; } = default!;
    public List<IDomainEvent> DomainEvents => [.. _domainEvents];

    #endregion

    #region Constructors
    protected Entity()
    {
    }

    protected Entity(TId id)
    {
        Id = id;
    }
    #endregion

    #region Domain Events
    public void AddDomainEvent(IDomainEvent domainEvent)
    {

        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    #endregion

    #region Equality
    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return GetType() == other.GetType() && Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals(obj as Entity<TId>);
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => left?.Equals(right) ?? right is null;
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
    #endregion
}
