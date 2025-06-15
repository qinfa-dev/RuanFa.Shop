using RuanFa.FashionShop.SharedKernel.Domains.Entities;

namespace RuanFa.FashionShop.SharedKernel.Domains.AggregateRoots;

public abstract class AggregateRoot<TId> : AuditableEntity<TId> where TId : notnull
{
    #region Constructors
    protected AggregateRoot() : base() { }

    protected AggregateRoot(TId id) : base(id) { }
    #endregion
}
