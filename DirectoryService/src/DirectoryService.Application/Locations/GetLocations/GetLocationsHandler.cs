using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Fails;

namespace DirectoryService.Application.Locations.GetLocations;

public class GetLocationsHandler
{
    private readonly IReadDbContext _readDbContext;
    private readonly IValidator<GetLocationQuery> _validator;

    public GetLocationsHandler(
        IReadDbContext readDbContext,
        IValidator<GetLocationQuery> validator)
    {
        _readDbContext = readDbContext;
        _validator = validator;
    }

    public async Task<Result<PaginationLocationResponse, Errors>> Handle(
        GetLocationQuery locationQuery,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(locationQuery, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToList();
        }

        var query = _readDbContext.LocationsRead;

        if (!string.IsNullOrWhiteSpace(locationQuery.Request.Search))
        {
            query = query.Where(l => EF.Functions.Like(l.Name.Value.ToLower(), $"%{locationQuery.Request.Search.ToLower()}%"));
        }

        if (locationQuery.Request.DepartmentIds is { Length: > 0 })
        {
            var departmentIds = locationQuery.Request.DepartmentIds
                .Select(id => new DepartmentId(id))
                .ToList();

            query = (from l in query
                join dl in _readDbContext.DepartmentLocationsRead on l.Id equals dl.LocationId
                where departmentIds.Contains(dl.DepartmentId)
                select l).Distinct();
        }

        if (locationQuery.Request.IsActive != null)
        {
            query = query.Where(l => l.IsActive == locationQuery.Request.IsActive);
        }

        Expression<Func<Location, object>> keySelector = locationQuery.Request.SortBy?.ToLower() switch
        {
            "name" => l => l.Name.Value,
            "date" => l => l.CreatedAt,
            _ => l => l.Name.Value
        };

        query = locationQuery.Request.SortOrder == "asc"
            ? query.OrderBy(keySelector)
            : query.OrderByDescending(keySelector);

        int locationsCount = await query.CountAsync(cancellationToken);

        var locations = await query
            .Select(l => new GetLocationDto
            {
                Name = l.Name.Value,
                County = l.Address.Country,
                City = l.Address.City,
                Street = l.Address.Street,
                HouseNumber = l.Address.HouseNumber,
                PostalCode = l.Address.PostalCode,
                TimeZone = l.TimeZone.Value,
                CreatedAt = l.CreatedAt,
                IsActive = l.IsActive,
            })
            .Skip((locationQuery.Request.Page - 1) * locationQuery.Request.PageSize ?? 0)
            .Take(locationQuery.Request.PageSize ?? 10)
            .ToListAsync(cancellationToken);

        return new PaginationLocationResponse(locations, locationsCount);
    }
}