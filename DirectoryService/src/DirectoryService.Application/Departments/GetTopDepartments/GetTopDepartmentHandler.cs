using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Departments;
using Microsoft.EntityFrameworkCore;
using Shared.Fails;

namespace DirectoryService.Application.Departments.GetTopDepartments;

public class GetTopDepartmentHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetTopDepartmentHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<TopDepartmentsResponse, Errors>> Handle(
        CancellationToken cancellationToken)
    {
        var query = from d in _readDbContext.DepartmentsRead
            join dp in _readDbContext.DepartmentPositionsRead on d.Id equals dp.DepartmentId
            group dp by d into g
            orderby g.Count() descending
            select new TopDepartmentDto
            {
                Id = g.Key.Id.Value,
                Name = g.Key.Name.Value,
                Identifier = g.Key.Identifier.Value,
                CreatedAt = g.Key.CreatedAt,
                IsActive = g.Key.IsActive,
                PositionCount = g.Count(),
            };

        var departments = await query
            .Take(5)
            .ToListAsync(cancellationToken);

        return new TopDepartmentsResponse(departments);
    }
}