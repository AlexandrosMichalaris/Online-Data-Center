using AutoMapper;
using FileProcessing.Model;
using StorageService.Model.Domain;

namespace DataCenter.Mapping;

public class FileRecordProfile : Profile
{
    public FileRecordProfile()
    {
        CreateMap<FileRecord, FileRecordEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
            .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.FileSize))
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Checksum, opt => opt.MapFrom(src => src.Checksum))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
            .ReverseMap();
    }
}