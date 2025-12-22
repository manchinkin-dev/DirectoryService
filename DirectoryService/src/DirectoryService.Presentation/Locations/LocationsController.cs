using DirectoryService.Application.Locations;
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
        [FromBody] CreateLocationDto locationDto,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(locationDto, cancellationToken);
    }
}