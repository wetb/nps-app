using NPSApplication.Domain.Entities;
using NPSApplication.Application.DTOs;
using AutoMapper;

namespace NPSApplication.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, AuthenticationResponse>()
            .ForMember(dest => dest.Role, 
                opt => opt.MapFrom(src => src.Role.ToString()));

        // Vote mappings
        CreateMap<Vote, VoteRequest>().ReverseMap();
        
        // NPS Result mappings
    }
}