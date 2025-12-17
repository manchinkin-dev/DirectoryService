using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_positions");

        builder
            .HasKey(dp => dp.Id)
            .HasName("pk_department_positions");

        builder
            .Property(dp => dp.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                dp => dp.Value,
                id => new DepartmentPositionId(id));

        builder
            .Property(dp => dp.DepartmentId)
            .IsRequired()
            .HasColumnName("department_id")
            .HasConversion(
                dp => dp.Value,
                id => new DepartmentId(id));

        builder
            .Property(dp => dp.PositionId)
            .IsRequired()
            .HasColumnName("position_id")
            .HasConversion(
                dp => dp.Value,
                id => new PositionId(id));
    }
}