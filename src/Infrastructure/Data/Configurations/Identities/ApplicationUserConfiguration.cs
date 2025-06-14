using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.FashionShop.Infrastructure.Accounts.Entities;

namespace RuanFa.FashionShop.Infrastructure.Data.Configurations.Identities;

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.RefreshToken)
            .HasMaxLength(500);

        builder.Property(ti => ti.Status)
            .HasConversion<int>();
    }
}
