using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Model.Dto;

namespace Data_Center.Configuration.Database;

public class HangfireStatusConfiguration: IEntityTypeConfiguration<HangfireStateDto>
{
    public void Configure(EntityTypeBuilder<HangfireStateDto> builder)
    {
        builder.ToTable("State", "Hangfire");
    }
}