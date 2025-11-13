using NPSApplication.Application.Common.Interfaces;
using NPSApplication.Application.DTOs;
using MediatR;

namespace NPSApplication.Application.Features.Votes.Queries;

public record GetNPSResultQuery : IRequest<NPSResultResponse>;

public class GetNPSResultQueryHandler 
    : IRequestHandler<GetNPSResultQuery, NPSResultResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNPSResultQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<NPSResultResponse> Handle(
        GetNPSResultQuery request, 
        CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Votes.GetNPSResultsAsync();

        var response = new NPSResultResponse
        {
            TotalVotes = result.TotalVotes,
            Promoters = result.Promoters,
            Neutrals = result.Neutrals,
            Detractors = result.Detractors,
            NPSScore = result.NPSScore
        };

        response.CalculatePercentages();

        return response;
    }
}