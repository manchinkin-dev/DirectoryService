using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder
            .HasKey(d => d.Id)
            .HasName("pk_departments");

        builder
            .Property(d => d.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                d => d.Value,
                id => new DepartmentId(id));

        builder
            .Property(d => d.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(150)
            .HasConversion(
                d => d.Value,
                name => DepartmentName.Create(name).Value);

        builder
            .Property(d => d.Identifier)
            .IsRequired()
            .HasColumnName("identifier")
            .HasMaxLength(150)
            .HasConversion(
                d => d.Value,
                identifier => Identifier.Create(identifier).Value);

        builder
            .Property(d => d.ParentId)
            .IsRequired(false)
            .HasColumnName("parent_id");

        builder
            .Property(d => d.Path)
            .IsRequired()
            .HasColumnName("path")
            .HasConversion(
                d => d.Value,
                path => DepartmentPath.Create(path).Value);

        builder
            .Property(d => d.Depth)
            .IsRequired()
            .HasColumnName("depth");

        builder
            .Property(d => d.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder
            .Property(d => d.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder
            .Property(d => d.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder
            .HasMany(d => d.Locations)
            .WithOne()
            .HasForeignKey(d => d.DepartmentId);

        builder
            .HasMany(d => d.Positions)
            .WithOne()
            .HasForeignKey(d => d.DepartmentId);

        builder
            .HasMany(d => d.Children)
            .WithOne()
            .IsRequired(false)
            .HasForeignKey(d => d.ParentId);
    }
}