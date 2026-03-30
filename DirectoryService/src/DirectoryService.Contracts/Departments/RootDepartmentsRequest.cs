namespace DirectoryService.Contracts.Departments;

public record RootDepartmentsRequest
{
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 20;
    public int Prefetch { get; init; } = 3;
}