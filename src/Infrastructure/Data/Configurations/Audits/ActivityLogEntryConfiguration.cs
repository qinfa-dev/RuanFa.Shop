using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.FashionShop.Application.Abstractions.Loggings.Entities;

namespace RuanFa.FashionShop.Infrastructure.Data.Configurations.Audits;

internal sealed class ActivityLogEntryConfiguration : IEntityTypeConfiguration<ActivityLogEntry>
{
    public void Configure(EntityTypeBuilder<ActivityLogEntry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Username)
            .HasMaxLength(200)
            .IsUnicode(false);

        builder.Property(e => e.Activity)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Endpoint)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.HttpMethod)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.Timestamp)
            .IsRequired();

        builder.Property(e => e.IpAddress)
            .IsRequired()
            .HasMaxLength(45); // Supports IPv6 length

        builder.Property(e => e.UserAgent)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.Result)
            .HasMaxLength(50);

        builder.Property(e => e.Request)
            .HasColumnType("TEXT");

        // Indexes for efficient queries
        builder.HasIndex(e => e.Timestamp);
        builder.HasIndex(e => e.Username);
        builder.HasIndex(e => e.Activity);
    }
}
