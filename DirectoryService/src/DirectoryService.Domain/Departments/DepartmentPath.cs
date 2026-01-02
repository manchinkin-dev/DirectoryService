namespace DirectoryService.Domain.Departments;

public record DepartmentPath
{
    private const char PATH_SEPERATOR = '.';
    private DepartmentPath(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static DepartmentPath CreateParent(Identifier identifier)
    {
        return new DepartmentPath(identifier.Value);
    }

    public DepartmentPath CreateChild(Identifier childIdentifier)
    {
        return new DepartmentPath(Value + PATH_SEPERATOR + childIdentifier.Value);
    }

    public static DepartmentPath Create(string value)
    {
        return new DepartmentPath(value);
    }
};