using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_locations");

        builder
            .HasKey(dl => dl.Id)
            .HasName("pk_department_locations");

        builder
            .Property(dl => dl.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                dl => dl.Value,
                id => new DepartmentLocationId(id));

        builder
            .Property(dl => dl.DepartmentId)
            .IsRequired()
            .HasColumnName("department_id")
            .HasConversion(
                dl => dl.Value,
                id => new DepartmentId(id));

        builder
            .Property(dl => dl.LocationId)
            .IsRequired()
            .HasColumnName("location_id")
            .HasConversion(
                dl => dl.Value,
                id => new LocationId(id));
    }
}