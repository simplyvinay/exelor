using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Auth.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Exelor.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUser _currentUser;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILoggerFactory _loggerFactory; 

        public ApplicationDbContext(
            DbContextOptions options,
            IConfiguration configuration,
            ICurrentUser currentUser,
            IPasswordHasher passwordHasher,
            ILoggerFactory loggerFactory)
            : base(options)
        {
            _configuration = configuration;
            _currentUser = currentUser;
            _passwordHasher = passwordHasher;
            _loggerFactory = loggerFactory;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            optionsBuilder.UseLoggerFactory(_loggerFactory);
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

            SeedData(builder);
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


        private void SeedData(
            ModelBuilder builder)
        {
            var createdAt = DateTime.Now;
            var salt = Guid.NewGuid().ToByteArray();
            var user1 = new User(
                "John",
                "Doe",
                "john@demo.com",
                "john",
                string.Empty,
                _passwordHasher.Hash(
                    "test",
                    salt),
                salt
            )
            {
                Id = 1,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                Archived = false
            };
            
            var role1 = new
            {
                Id = 1,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                Archived = false,
                Name = "Base User",
                _permissionsInRole = new List<Permissions>{ Permissions.ReadUsers }.PackPermissions()
            };
            
            builder.Entity<UserRole>().HasData(
                new
                {
                    UserId = 1,
                    RoleId = 1
                }
            );

            builder.Entity<User>().HasData(user1);
            builder.Entity<Role>().HasData(role1);
            
            var user2 = new User(
                "Jane",
                "Doe",
                "jane@demo.com",
                "jane",
                string.Empty,
                _passwordHasher.Hash(
                    "test",
                    salt),
                salt
            )
            {
                Id = 2,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                Archived = false
            };
            
            var role2 = new
            {
                Id = 2,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                Archived = false,
                Name = "Base+ User",
                _permissionsInRole = new List<Permissions>{ Permissions.EditUsers }.PackPermissions()
            };
            
            builder.Entity<UserRole>().HasData(
                new
                {
                    UserId = 2,
                    RoleId = 2
                }
            );

            builder.Entity<User>().HasData(user2);
            builder.Entity<Role>().HasData(role2);
        }
    }
}