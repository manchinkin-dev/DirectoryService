using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Departments.GetRootDepartments;

public record GetRootDepartmentQuery(
    RootDepartmentsRequest Request);