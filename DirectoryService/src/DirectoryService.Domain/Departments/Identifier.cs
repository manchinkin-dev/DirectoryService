using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Shared;
using Shared.Errors;

namespace DirectoryService.Domain.Departments;

public partial record Identifier
{
    private const int IDENTIFIER_MIN_LENGTH = 3;
    private const int IDENTIFIER_MAX_LENGTH = 150;
    private static readonly Regex _regex = IdentifierRegex();

    private Identifier(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Identifier, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsRequired("идентификатор подразделения");
        }

        if (value.Length < IDENTIFIER_MIN_LENGTH || value.Length > IDENTIFIER_MAX_LENGTH || !_regex.IsMatch(value))
        {
            return GeneralErrors.ValueIsInvalid("идентификатор подразделения");
        }

        return new Identifier(value);
    }

    [GeneratedRegex("^a-zA-Z+$")]
    private static partial Regex IdentifierRegex();
};