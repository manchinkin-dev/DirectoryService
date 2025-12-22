using CSharpFunctionalExtensions;
using Shared;
using Shared.Errors;

namespace DirectoryService.Domain.Positions;

public record PositionName
{
    private const int POSITION_NAME_MIN_LENGTH = 3;
    private const int POSITION_NAME_MAX_LENGTH = 100;

    private PositionName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PositionName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsRequired("наименование должности");
        }

        if (value.Length < POSITION_NAME_MIN_LENGTH || value.Length > POSITION_NAME_MAX_LENGTH)
        {
            return GeneralErrors.ValueIsInvalid("наименование должности");
        }

        return new PositionName(value);
    }
};