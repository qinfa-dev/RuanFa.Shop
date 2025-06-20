﻿using RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

namespace RuanFa.FashionShop.SharedKernel.Domains.Entities;

public abstract class AuditableEntity<TId> : Entity<TId>, IAuditable where TId : notnull
{
    #region Constructors
    protected AuditableEntity()
    {
    }

    protected AuditableEntity(TId id) : base(id)
    {
        Id = id;
    }
    #endregion
    #region Properties
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    #endregion
}
