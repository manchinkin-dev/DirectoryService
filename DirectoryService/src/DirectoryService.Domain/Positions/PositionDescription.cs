using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Positions;

public record PositionDescription
{
    private const int POSITION_DESCRIPTION_MAX_LENGTH = 1000;

    private PositionDescription(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PositionDescription, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsRequired("описание должности");
        }

        if (value.Length > POSITION_DESCRIPTION_MAX_LENGTH)
        {
            return GeneralErrors.ValueIsInvalid("описание должности");
        }

        return new PositionDescription(value);
    }
};