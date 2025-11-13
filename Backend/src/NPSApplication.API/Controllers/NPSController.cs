using MediatR;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using NPSApplication.Application.DTOs;

using NPSApplication.Application.Features.Votes.Queries;

namespace NPSApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class NPSController : ControllerBase
{
    private readonly IMediator _mediator;

    public NPSController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("results")]
    public async Task<ActionResult<NPSResultResponse>> GetResults()
    {
        var query = new GetNPSResultQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}