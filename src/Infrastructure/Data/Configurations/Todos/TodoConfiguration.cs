using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.FashionShop.Domain.Todos.AggregateRoot;
using RuanFa.FashionShop.Infrastructure.Data.Constants;
using RuanFa.FashionShop.Infrastructure.Data.Converters;

namespace RuanFa.FashionShop.Infrastructure.Data.Configurations.Todos;

internal sealed class TodoConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.ToTable(Schema.TodoLists);

        // Configure primary key
        builder.HasKey(t => t.Id);

        // Configure properties
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(t => t.Colour)
            .HasValueJsonConverter()
            .HasColumnType("TEXT");

        builder.HasMany(t => t.Items)
            .WithOne(m => m.List)
            .HasForeignKey(ti => ti.ListId);
    }
}
