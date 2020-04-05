using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiStarter.Domain.Identity;
using ApiStarter.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApiStarter.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        User,
        Role,
        int,
        IdentityUserClaim<int>,
        UserRole,
        IdentityUserLogin<int>,
        IdentityRoleClaim<int>,
        IdentityUserToken<int>>
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

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            //optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("User");
            
            builder.Entity<Role>().ToTable("Role");
            builder.Entity<Role>().HasMany(a => a.Users).WithOne(a => a.Role).HasForeignKey(a => a.RoleId);
            builder.Entity<Role>()
                .Property("_permissionsInRole")
                .HasColumnName("PermissionsInRole")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            
            builder.Entity<UserRole>()
                .ToTable("UserRole")
                .HasKey(r => new {r.UserId, r.RoleId});
            
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaim");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogin");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserToken");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaim");
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