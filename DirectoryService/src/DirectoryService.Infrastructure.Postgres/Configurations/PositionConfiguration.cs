using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder
            .HasKey(p => p.Id)
            .HasName("pk_positions");

        builder
            .Property(p => p.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                p => p.Value,
                id => new PositionId(id));

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(
                p => p.Value,
                name => PositionName.Create(name).Value)
            .HasColumnName("name");
        builder.HasIndex(p => p.Name).IsUnique();

        builder
            .Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000)
            .HasConversion(
                p => p.Value,
                description => PositionDescription.Create(description).Value)
            .HasColumnName("description");

        builder
            .Property(p => p.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder
            .Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder
            .Property(p => p.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder
            .HasMany(p => p.Positions)
            .WithOne()
            .HasForeignKey(p => p.PositionId);
    }
}