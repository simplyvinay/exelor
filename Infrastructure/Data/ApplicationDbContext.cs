using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Infrastructure.Auth.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Exelor.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private ICurrentUser _currentUser;

        public ApplicationDbContext(
            DbContextOptions options,
            IConfiguration configuration,
            ICurrentUser currentUser)
            : base(options)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        //public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            //optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            var navigation = builder.Entity<User>()
                .Metadata.FindNavigation(nameof(User.RefreshTokens));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Entity<RefreshToken>()
                .HasOne(d => d.User)
                .WithMany(e => e.RefreshTokens)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);;

            builder.Entity<Role>()
                .HasMany(a => a.Users)
                .WithOne(a => a.Role)
                .HasForeignKey(a => a.RoleId);

            builder.Entity<Role>()
                .Property("_permissionsInRole")
                .HasColumnName("PermissionsInRole")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            
            builder.Entity<UserRole>()
                .ToTable("UserRole")
                .HasKey(r => new {r.UserId, r.RoleId});
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        private void UpdateAuditFields()
        {
            // get entries that are being Added or Updated
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                var entity = entry.Entity as Entity;

                if (entity != null)
                {
                    var now = DateTime.Now;
                    var userId = _currentUser.Id;

                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedBy = userId;
                        entity.CreatedAt = now;
                    }

                    entity.UpdatedBy = userId;
                    entity.UpdatedAt = now;
                }
            }
        }
    }
}