using NPSApplication.Application.Common.Interfaces;
using MediatR;

namespace NPSApplication.Application.Features.Votes.Queries;

public record CheckUserVoteQuery(int UserId) : IRequest<bool>;

public class CheckUserVoteQueryHandler 
    : IRequestHandler<CheckUserVoteQuery, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckUserVoteQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        CheckUserVoteQuery request, 
        CancellationToken cancellationToken)
    {
        return await _unitOfWork.Votes.HasUserVotedAsync(request.UserId);
    }
}