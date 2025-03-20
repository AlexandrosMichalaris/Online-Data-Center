using AutoMapper;
using StorageService.Model.Domain;
using StorageService.Model.Entities;

namespace DataCenter.Mapping;

public class JobFileRecordProfile : Profile
{
    public JobFileRecordProfile()
    {
        CreateMap<JobFileRecord, JobFileRecordEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.FileId, opt => opt.MapFrom(src => src.FileId))
            .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.JobId))
            .ForMember(dest => dest.ScheduledAt, opt => opt.MapFrom(src => src.ScheduledAt))
            .ReverseMap();
    }
}