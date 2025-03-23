using AutoMapper;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using StorageService.Model.Domain;
using StorageService.Model.Entities;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.DomainRepository;

public class HangfireJobDomainRepository: DomainRepository<HangfireJobEntity, HangfireJob>, IHangfireJobDomainRepository
{
    public HangfireJobDomainRepository(IHangfireJobEntityRepository hangfireJobEntityRepository, IMapper mapper) : base(hangfireJobEntityRepository, mapper) { }
}