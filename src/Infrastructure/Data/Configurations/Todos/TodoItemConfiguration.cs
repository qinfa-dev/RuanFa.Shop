﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Domain.Todos.Entities;
using RuanFa.FashionShop.Infrastructure.Data.Constants;

namespace RuanFa.FashionShop.Infrastructure.Data.Configurations.Todos;

internal sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable(Schema.TodoItems);

        // Configure primary key (inherited from BaseAuditableEntity)
        builder.HasKey(ti => ti.Id);

        // Configure properties
        builder.Property(ti => ti.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ti => ti.Note)
            .HasMaxLength(500);

        builder.Property(ti => ti.Priority)
            .HasConversion<int>();

        builder.Property(ti => ti.Done)
            .IsRequired();

        builder.Property(ti => ti.DoneAt)
            .IsRequired(false);

        // Configure relationships
        builder.HasOne(ti => ti.List)
            .WithMany(m => m.Items)
            .HasForeignKey(ti => ti.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        // Optionally: Configure indexes
        builder.HasIndex(ti => ti.ListId);
    }
}
