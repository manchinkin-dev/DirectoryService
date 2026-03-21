namespace DirectoryService.Contracts.Locations;

public record GetLocationRequest(
    Guid[]? DepartmentIds,
    bool? IsActive,
    int? Page,
    int? PageSize,
    string? Search,
    string? SortBy,
    string? SortOrder);