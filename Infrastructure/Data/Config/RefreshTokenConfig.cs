using Exelor.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exelor.Infrastructure.Data.Config
{
    public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(
            EntityTypeBuilder<RefreshToken> builder)
        {
            builder
                .HasOne(d => d.User)
                .WithMany(e => e.RefreshTokens)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}