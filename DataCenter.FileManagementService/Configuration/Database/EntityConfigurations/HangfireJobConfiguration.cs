// HangfireJobConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Model.Dto;

namespace Data_Center.Configuration.Database;

public class HangfireJobConfiguration : IEntityTypeConfiguration<HangfireJobDto>
{
    public void Configure(EntityTypeBuilder<HangfireJobDto> builder)
    {
        builder.ToTable("Job", "Hangfire");
    }
}