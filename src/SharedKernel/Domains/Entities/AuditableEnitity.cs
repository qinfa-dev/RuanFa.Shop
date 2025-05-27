namespace RuanFa.FashionShop.SharedKernel.Domains.Entities;

public abstract class AuditableEnitity<TId> : Entity<TId> where TId : notnull
{
    #region Constructors
    protected AuditableEnitity()
    {
    }

    protected AuditableEnitity(TId id) : base(id)
    {
        Id = id;
    }
    #endregion
    #region Properties
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    #endregion
}
