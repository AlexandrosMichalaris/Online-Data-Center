using DataCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data_Center.Configuration.Database;

public class TrustedIpConfiguration : IEntityTypeConfiguration<TrustedIpEntity>
{
    public void Configure(EntityTypeBuilder<TrustedIpEntity> builder)
    {
        builder.ToTable("trustedip");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");
        
        builder.Property(x => x.IpAddress)
            .HasColumnName("ipaddress");
        
        builder.Property(x => x.UserId)
            .HasColumnName("userid");
        
        builder.Property(x => x.CreatedAt)
            .HasColumnName("createdat");
        
        builder
            .HasOne(t => t.UserEntity)
            .WithMany(u => u.TrustedIps)
            .HasForeignKey(t => t.UserId);
    }
}