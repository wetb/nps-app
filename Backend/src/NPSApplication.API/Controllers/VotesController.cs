using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPSApplication.Application.DTOs;
using System.Security.Claims;

using NPSApplication.Application.Features.Votes.Queries;
using NPSApplication.Application.Features.Votes.Commands;

namespace NPSApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Voter")]
    public async Task<ActionResult> CreateVote([FromBody] VoteRequest request)
    {
        var userId = int.Parse(User.FindFirstValue("userId")!);
        
        var command = new CreateVoteCommand(userId, request.Score);
        var voteId = await _mediator.Send(command);
        
        return Ok(new { 
            message = "Voto registrado exitosamente", 
            voteId 
        });
    }

    [HttpGet("check")]
    [Authorize(Roles = "Voter")]
    public async Task<ActionResult<bool>> CheckVote()
    {
        var userId = int.Parse(User.FindFirstValue("userId")!);
        
        var query = new CheckUserVoteQuery(userId);
        var hasVoted = await _mediator.Send(query);
        
        return Ok(new { hasVoted });
    }
}