using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Locations;

public record TimeZone
{
    private TimeZone(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<TimeZone, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsRequired("IANA‑код");
        }

        if (!TimeZoneInfo.TryFindSystemTimeZoneById(value, out var timeZone))
        {
            return GeneralErrors.ValueIsInvalid("IANA‑код");
        }

        return new TimeZone(value);
    }
};