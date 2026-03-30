namespace DirectoryService.Contracts.Departments;

public record DepartmentChildrenRequest
{
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 20;
};