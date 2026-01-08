using DirectoryService.Application.Departments.CreateDepartment;
using DirectoryService.Application.Departments.MoveDepartment;
using DirectoryService.Application.Departments.UpdateDepartmentLocations;
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

     [HttpPatch("{departmentId:guid}/locations")]
     public async Task<EndpointResult<Guid>> UpdateLocations(
         [FromRoute] Guid departmentId,
         [FromBody] UpdateDepartmentLocationsRequest request,
         [FromServices] UpdateDepartmentLocationsHandler handler,
         CancellationToken cancellationToken)
     {
         var command = new UpdateDepartmentLocationsCommand(departmentId, request);

         return await handler.Handle(command, cancellationToken);
     }

     [HttpPatch("{departmentId:guid}/parent")]
     public async Task<EndpointResult<Guid>> MoveDepartment(
         [FromRoute] Guid departmentId,
         [FromBody] MoveDepartmentRequest request,
         [FromServices] MoveDepartmentHandler handler,
         CancellationToken cancellationToken)
     {
         var command = new MoveDepartmentCommand(departmentId, request);

         return await handler.Handle(command, cancellationToken);
     }
}