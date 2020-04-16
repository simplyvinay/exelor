using Exelor.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exelor.Infrastructure.Data.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(
            EntityTypeBuilder<User> builder)
        {
            var navigation = builder
                .Metadata.FindNavigation(nameof(User.RefreshTokens));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder
                .HasMany(a => a.Roles)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

        }
    }
}