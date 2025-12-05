using BusTrackBackEnd.API.Routes.Domain.Model.Queries;
using BusTrackBackEnd.API.Routes.Domain.Services;
using BusTrackBackEnd.API.Routes.Interfaces.REST.Resources;
using BusTrackBackEnd.API.Routes.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using BusTrackBackEnd.API.Routes.Application.Internal.CommandServices;
using BusTrackBackEnd.API.Routes.Application.Internal.QueryServices;

namespace BusTrackBackEnd.API.Routes.Interfaces.REST;

[ApiController]
[Route("api/v1/routes")]
public class RoutesController : ControllerBase
{
    private readonly IRouteCommandService _commandService;
    private readonly IRouteQueryService _queryService;

    public RoutesController(
        IRouteCommandService commandService,
        IRouteQueryService queryService)
    {
        _commandService = commandService;
        _queryService = queryService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRouteResource resource)
    {
        var command = CreateRouteCommandFromResourceAssembler.ToCommand(resource);
        var id = await _commandService.Handle(command);
        return Ok(new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _queryService.Handle(new GetAllRoutesQuery());
        return Ok(list.Select(RouteResourceFromEntityAssembler.ToResource));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var route = await _queryService.Handle(new GetRouteByIdQuery(id));
        if (route == null) return NotFound();
        return Ok(RouteResourceFromEntityAssembler.ToResource(route));
    }
}