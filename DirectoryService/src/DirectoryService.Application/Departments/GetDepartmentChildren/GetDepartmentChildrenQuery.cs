using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Departments.GetDepartmentChildren;

public record GetDepartmentChildrenQuery(
    Guid ParentId,
    DepartmentChildrenRequest Request);