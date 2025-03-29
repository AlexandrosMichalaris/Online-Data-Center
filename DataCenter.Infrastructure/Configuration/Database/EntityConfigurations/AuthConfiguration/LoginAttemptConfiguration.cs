using DataCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data_Center.Configuration.Database;

public class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttemptEntity>
{
    public void Configure(EntityTypeBuilder<LoginAttemptEntity> builder)
    {
        builder.ToTable("loginattempt");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("id");
        
        builder.Property(x => x.UserId)
            .HasColumnName("userid");
        
        builder.Property(x => x.IpAddress)
            .HasColumnName("ipaddress");
        
        builder.Property(x => x.Success)
            .HasColumnName("success");
        
        builder.Property(x => x.AttemptAt)
            .HasColumnName("attemptedat");
        
        builder
            .HasOne(l => l.UserEntity)
            .WithMany()
            .HasForeignKey(l => l.UserId);
    }
}