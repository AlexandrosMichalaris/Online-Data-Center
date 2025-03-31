using AutoMapper;
using DataCenter.Domain.Domain;
using DataCenter.Domain.Entities;

namespace DataCenter.Mapping;

public class LoginAttemptProfile : Profile
{
    public LoginAttemptProfile()
    {
        CreateMap<LoginAttempt, LoginAttemptEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IpAddress))
            .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
            .ForMember(dest => dest.AttemptAt, opt => opt.MapFrom(src => src.AttemptedAt))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
            .ReverseMap();
    }
}