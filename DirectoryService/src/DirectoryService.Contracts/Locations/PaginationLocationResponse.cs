namespace DirectoryService.Contracts.Locations;

public record PaginationLocationResponse(
    IReadOnlyList<GetLocationDto> Locations,
    int TotalCount);