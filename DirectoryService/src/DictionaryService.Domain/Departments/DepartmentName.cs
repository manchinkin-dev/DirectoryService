using CSharpFunctionalExtensions;
using DictionaryService.Domain.Shared;

namespace DictionaryService.Domain.Departments;

public record DepartmentName
{
    private const int DEPARTMENT_NAME_MIN_LENGTH = 3;
    private const int DEPARTMENT_NAME_MAX_LENGTH = 150;

    private DepartmentName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<DepartmentName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsRequired("наименование подразделения");
        }

        if (value.Length < DEPARTMENT_NAME_MIN_LENGTH || value.Length > DEPARTMENT_NAME_MAX_LENGTH)
        {
            return GeneralErrors.ValueIsInvalid("наименование подразделения");
        }

        return new DepartmentName(value);
    }
};