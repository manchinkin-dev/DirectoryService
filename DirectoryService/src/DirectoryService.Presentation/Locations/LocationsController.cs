using DirectoryService.Application.Locations.CreateLocation;
using DirectoryService.Application.Locations.GetLocations;
using DirectoryService.Contracts.Locations;
using DirectoryService.Presentation.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Locations;

[ApiController]
[Route("/api/[controller]")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] CreateLocationHandler handler,
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpGet]
    public async Task<EndpointResult<PaginationLocationResponse>> Get(
        [FromQuery] GetLocationRequest request,
        [FromServices] GetLocationsHandler handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(new GetLocationQuery(request), cancellationToken);
    }
}