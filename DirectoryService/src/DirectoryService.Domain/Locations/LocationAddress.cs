using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Locations;

public record LocationAddress
{
    private const int COUNTRY_MAX_LENGTH = 60;
    private const int CITY_MAX_LENGTH = 100;
    private const int HOUSE_NUMBER_MAX_LENGTH = 20;
    private const int STREET_MAX_LENGTH = 200;
    private const int POSTAL_MAX_LENGTH = 12;

    private LocationAddress(
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

    public static Result<LocationAddress, Error> Create(string country, string city, string street, string houseNumber, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return GeneralErrors.ValueIsRequired("страна");
        }

        if (country.Length > COUNTRY_MAX_LENGTH)
        {
            return GeneralErrors.ValueIsInvalid("страна");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return GeneralErrors.ValueIsRequired("город");
        }

        if (city.Length > CITY_MAX_LENGTH)
        {
            return GeneralErrors.ValueIsInvalid("город");
        }

        if (string.IsNullOrWhiteSpace(street))
        {
            return GeneralErrors.ValueIsRequired("улица");
        }

        if (street.Length > STREET_MAX_LENGTH)
        {
            return GeneralErrors.ValueIsInvalid("лица");
        }

        if (string.IsNullOrWhiteSpace(houseNumber))
        {
            return GeneralErrors.ValueIsRequired("номер дома");
        }

        if (houseNumber.Length > HOUSE_NUMBER_MAX_LENGTH)
        {
            return GeneralErrors.ValueIsInvalid("номер дома");
        }

        if (postalCode.Length > POSTAL_MAX_LENGTH)
        {
            return GeneralErrors.ValueIsInvalid("почтовый индекс");
        }

        return new LocationAddress(country, city, street, houseNumber, postalCode);
    }
};