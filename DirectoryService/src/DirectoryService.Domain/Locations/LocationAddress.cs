using System.Text.Json.Serialization;

namespace DirectoryService.Domain.Locations;

public record LocationAddress
{
    public LocationAddress(
        string country,
        string city,
        string street,
        string houseNumber,
        string postalCode)
    {
        Country = country;
        City = city;
        Street = street;
        HouseNumber = houseNumber;
        PostalCode = postalCode;
    }

    [JsonIgnore]
    public string FullAddress => $"{PostalCode}, {Country}, {City}, {Street}, {HouseNumber}";

    public string City { get; }
    public string Country { get; }
    public string Street { get; }
    public string HouseNumber { get; }
    public string PostalCode { get; }
};