using System.Text.Json;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeZone = DirectoryService.Domain.Locations.TimeZone;

namespace DirectoryService.Infrastructure.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder
            .HasKey(l => l.Id)
            .HasName("pk_locations");

        builder
            .Property(l => l.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                l => l.Value,
                id => new LocationId(id));

        builder.OwnsOne(l => l.Name, name =>
        {
            name.Property(x => x.Value)
                .HasColumnName("name")
                .HasMaxLength(120)
                .IsRequired();

            name.HasIndex(x => x.Value)
                .IsUnique();
        });

        builder
            .Property(l => l.Address)
            .HasColumnName("address")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<LocationAddress>(v, (JsonSerializerOptions?)null)!);
        builder.HasIndex(l => l.Address).IsUnique();

        builder
            .Property(l => l.TimeZone)
            .IsRequired()
            .HasColumnName("timezone")
            .HasConversion(
                l => l.Value,
                timeZone => TimeZone.Create(timeZone).Value);

        builder
            .Property(l => l.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder
            .Property(l => l.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder
            .Property(l => l.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder
            .HasMany(l => l.Departments)
            .WithOne()
            .HasForeignKey(l => l.LocationId);
    }
}