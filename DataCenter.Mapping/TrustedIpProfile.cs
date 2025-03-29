using AutoMapper;
using DataCenter.Domain.Domain;
using DataCenter.Domain.Entities;

namespace DataCenter.Mapping;

public class TrustedIpProfile : Profile
{
    public TrustedIpProfile()
    {
        CreateMap<TrustedIp, TrustedIpEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IpAddress))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ReverseMap();
    }
}