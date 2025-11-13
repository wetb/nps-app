using NPSApplication.Domain.Entities;
using NPSApplication.Domain.Enums;
using NPSApplication.Application.Common.Interfaces;
using MediatR;

namespace NPSApplication.Application.Features.Votes.Commands;

public record CreateVoteCommand(int UserId, int Score) 
    : IRequest<int>;

public class CreateVoteCommandHandler : IRequestHandler<CreateVoteCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateVoteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(
        CreateVoteCommand request, 
        CancellationToken cancellationToken)
    {
        // Verificar si el usuario ya votó
        var hasVoted = await _unitOfWork.Votes.HasUserVotedAsync(request.UserId);
        
        if (hasVoted)
            throw new InvalidOperationException("El usuario ya ha emitido su voto");

        // Verificar que el usuario existe y es votante
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null || user.Role != UserRole.Voter)
            throw new UnauthorizedAccessException("Usuario no autorizado para votar");

        // Crear voto
        var vote = new Vote
        {
            UserId = request.UserId,
            Score = request.Score,
            VotedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        vote.CalculateCategory();

        var voteId = await _unitOfWork.Votes.CreateAsync(vote);
        
        return voteId;
    }
}