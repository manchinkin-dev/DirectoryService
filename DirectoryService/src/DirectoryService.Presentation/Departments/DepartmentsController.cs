using DirectoryService.Application.Departments;
using DirectoryService.Application.Departments.CreateDepartment;
using DirectoryService.Contracts.Departments;
using DirectoryService.Presentation.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Departments;

[ApiController]
[Route("/api/[controller]")]
public class DepartmentsController : ControllerBase
{
     [HttpPost]
     public async Task<EndpointResult<Guid>> Create(
         [FromBody] CreateDepartmentRequest request,
         [FromServices] CreateDepartmentHandler handler,
         CancellationToken cancellationToken)
     {
         var command = new CreateDepartmentCommand(request);

         return await handler.Handle(command, cancellationToken);
     }
}