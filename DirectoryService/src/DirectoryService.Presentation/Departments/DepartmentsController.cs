using DirectoryService.Application.Departments.CreateDepartment;
using DirectoryService.Application.Departments.GetDepartmentChildren;
using DirectoryService.Application.Departments.GetRootDepartments;
using DirectoryService.Application.Departments.GetTopDepartments;
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

     [HttpGet("top-positions")]
     public async Task<EndpointResult<TopDepartmentsResponse>> GetTopDepartments(
         [FromServices] GetTopDepartmentHandler handler,
         CancellationToken cancellationToken)
     {
         return await handler.Handle(cancellationToken);
     }

     [HttpGet("roots")]
     public async Task<EndpointResult<RootDepartmentsResponse>> GetRootDepartments(
         [FromQuery] RootDepartmentsRequest request,
         [FromServices] GetRootDepartmentsHandler handler,
         CancellationToken cancellationToken)
     {
         var query = new GetRootDepartmentQuery(request);
         return await handler.Handle(query, cancellationToken);
     }

     [HttpGet("{parentId:guid}/children")]
     public async Task<EndpointResult<DepartmentChildrenResponse>> GetDepartmentChildren(
         [FromRoute] Guid parentId,
         [FromQuery] DepartmentChildrenRequest request,
         [FromServices] GetDepartmentChildrenHandler handler,
         CancellationToken cancellationToken)
     {
         var query = new GetDepartmentChildrenQuery(parentId, request);
         return await handler.Handle(query, cancellationToken);
     }
}