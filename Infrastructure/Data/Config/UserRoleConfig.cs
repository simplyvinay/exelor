using Exelor.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exelor.Infrastructure.Data.Config
{
    public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(
            EntityTypeBuilder<UserRole> builder)
        {
            builder
                .ToTable("userroles")
                .HasKey(r => new {r.UserId, r.RoleId});
        }
    }
}