using CSharpFunctionalExtensions;
using DictionaryService.Domain.Shared;

namespace DictionaryService.Domain.Locations;

public record LocationName
{
    private const int LOCATION_NAME_MIN_LENGTH = 3;
    private const int LOCATION_NAME_MAX_LENGTH = 120;

    private LocationName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<LocationName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsRequired("наименование локации");
        }

        if (value.Length < LOCATION_NAME_MIN_LENGTH || value.Length > LOCATION_NAME_MAX_LENGTH)
        {
            return GeneralErrors.ValueIsInvalid("наименование локации");
        }

        return new LocationName(value);
    }
};