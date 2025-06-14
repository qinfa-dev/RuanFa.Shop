using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.FashionShop.Infrastructure.Data.Constants;

namespace RuanFa.FashionShop.Infrastructure.Data.Configurations.Identities;

internal class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        builder.ToTable(Schema.UserLogins);
    }
}
