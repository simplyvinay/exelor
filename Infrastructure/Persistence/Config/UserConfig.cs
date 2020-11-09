using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(
            EntityTypeBuilder<User> builder)
        {
            var navigation = builder
                .Metadata.FindNavigation(nameof(User.RefreshTokens));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsOne(u => u.Address);
            builder.Property(u => u.CustomFields)
                .HasColumnType("jsonb");
            builder
                .HasMany(u => u.Roles)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserId);

        }
    }
}