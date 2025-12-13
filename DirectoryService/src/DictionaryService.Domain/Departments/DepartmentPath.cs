using CSharpFunctionalExtensions;
using DictionaryService.Domain.Shared;

namespace DictionaryService.Domain.Departments;

public record DepartmentPath
{
    private DepartmentPath(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<DepartmentPath, Error> Create(string value)
    {
        return new DepartmentPath(value);
    }
};