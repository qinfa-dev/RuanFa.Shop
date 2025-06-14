using Microsoft.AspNetCore.Identity;
using RuanFa.FashionShop.Domain.Accounts.Enums;
using RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

namespace RuanFa.FashionShop.Infrastructure.Accounts.Entities;
public class ApplicationUser : IdentityUser<Guid>, IAuditable
{
    #region Properties
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }

    public AccountStatus Status { get; set; }

    // Adudits
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    #endregion
}
