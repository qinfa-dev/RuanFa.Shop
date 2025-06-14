using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.FashionShop.Domain.Accounts.Entities;
using RuanFa.FashionShop.Infrastructure.Data.Constants;
using RuanFa.FashionShop.Infrastructure.Data.Converters;

namespace RuanFa.FashionShop.Infrastructure.Data.Configurations.Identities;

internal class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable(Schema.UserProfile);
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.UserId).IsUnique();
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.FullName).IsRequired();
        builder.Property(e => e.Gender).IsRequired();
        builder.Property(e => e.LoyaltyPoints).IsRequired();
        builder.Property(e => e.MarketingConsent).IsRequired();

        // Value converter for Addresses 
        builder.Property(up => up.Addresses)
            .HasValueJsonConverter()
            .HasColumnType("TEXT");

        // Value converter for Preferences
        builder.Property(up => up.Preferences)
             .HasValueJsonConverter()
             .HasColumnType("TEXT");

        // Value converter for Wishlist 
        builder.Property(e => e.Wishlist)
            .HasValueJsonConverter()
            .HasColumnType("TEXT");

        // Configure one-to-many with Order 
        //builder.HasMany(e => e.Orders)
        //    .WithOne(e => e.Profile)
        //    .HasForeignKey(e => e.Id)
        //    .OnDelete(DeleteBehavior.SetNull);
    }
}
