namespace DirectoryService.Contracts.Locations;

public record GetLocationDto
{
    public string Name { get; init; } = string.Empty;
    public string HouseNumber { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string County { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string TimeZone { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
};