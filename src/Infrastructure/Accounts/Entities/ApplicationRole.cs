using Microsoft.AspNetCore.Identity;
using RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

namespace RuanFa.FashionShop.Infrastructure.Accounts.Entities;
public class ApplicationRole : IdentityRole<Guid>, IAuditable
{
    public ApplicationRole()
    {
    }
    public ApplicationRole(string roleName) : base(roleName)
    {
    }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}
