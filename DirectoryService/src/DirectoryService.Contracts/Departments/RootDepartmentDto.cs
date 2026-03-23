namespace DirectoryService.Contracts.Departments;

public record RootDepartmentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid? ParentId { get; init; }
    public string Identifier { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public int ChildrenCount { get; init; }
    public bool HasMoreChildren { get; init; }
}