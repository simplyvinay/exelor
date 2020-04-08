﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Auth.Authorization;
using Exelor.Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Exelor.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUser _currentUser;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILoggerFactory _loggerFactory;
        private readonly AuditSettings _auditSettings;

        public ApplicationDbContext(
            DbContextOptions options,
            ICurrentUser currentUser,
            IPasswordHasher passwordHasher,
            ILoggerFactory loggerFactory,
            IOptions<AuditSettings> auditSettings)
            : base(options)
        {
            _currentUser = currentUser;
            _passwordHasher = passwordHasher;
            _loggerFactory = loggerFactory;
            _auditSettings = auditSettings.Value;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Audit> Audits { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
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
                .OnDelete(DeleteBehavior.Cascade);
            ;

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

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            var auditEntries = await OnBeforeSaveChanges();
            UpdateAuditFieldsOnEntities();
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        public override int SaveChanges()
        {
            var auditEntries = Task.Run(OnBeforeSaveChanges).Result;
            UpdateAuditFieldsOnEntities();
            var result = base.SaveChanges();
            Task.Run(() => OnAfterSaveChanges(auditEntries));
            return result;
        }

        async Task<IEnumerable<(EntityEntry EntityEntry, Audit Audit)>> OnBeforeSaveChanges()
        {
            if (!_auditSettings.Enabled || _auditSettings.Sink != AuditSink.Database)
                return null;

            ChangeTracker.DetectChanges();
            var entitiesToTrack = ChangeTracker.Entries().Where(
                e => !(e.Entity is Audit) && e.State != EntityState.Detached && e.State != EntityState.Unchanged);

            foreach (var entityEntry in entitiesToTrack.Where(e => !e.Properties.Any(p => p.IsTemporary)))
            {
                var auditExcludedProps = entityEntry.Entity.GetType()
                    .GetProperties()
                    .Where(
                        p => p.GetCustomAttributes(
                            typeof(JsonIgnoreAttribute),
                            false).Any())
                    .Select(p => p.Name)
                    .ToList();


                await Audits.AddRangeAsync(
                    new Audit()
                    {
                        Table = entityEntry.Metadata.GetTableName(),
                        Date = DateTime.Now.ToUniversalTime(),
                        User = _currentUser.Name,
                        KeyValues = JsonSerializer.Serialize(
                            entityEntry.Properties.Where(p => p.Metadata.IsPrimaryKey()).ToDictionary(
                                p => p.Metadata.Name,
                                p => p.CurrentValue)),
                        NewValues = JsonSerializer.Serialize(
                            entityEntry.Properties.Where(
                                    p => entityEntry.State == EntityState.Added ||
                                         entityEntry.State == EntityState.Modified &&
                                         !auditExcludedProps.Contains(p.Metadata.Name))
                                .ToDictionary(
                                    p => p.Metadata.Name,
                                    p => p.CurrentValue)),
                        OldValues = JsonSerializer.Serialize(
                            entityEntry.Properties.Where(
                                    p => entityEntry.State == EntityState.Deleted ||
                                         entityEntry.State == EntityState.Modified &&
                                         !auditExcludedProps.Contains(p.Metadata.Name))
                                .ToDictionary(
                                    p => p.Metadata.Name,
                                    p => p.OriginalValue))
                    });
            }

            var returnList = new List<(EntityEntry EntityEntry, Audit Audit)>();
            foreach (var entityEntry in entitiesToTrack.Where(e => e.Properties.Any(p => p.IsTemporary)))
            {
                var auditExcludedProps = entityEntry.Entity.GetType()
                    .GetProperties()
                    .Where(
                        p => p.GetCustomAttributes(
                            typeof(JsonIgnoreAttribute),
                            false).Any())
                    .Select(p => p.Name)
                    .ToList();

                returnList.Add(
                    (entityEntry,
                        new Audit()
                        {
                            Table = entityEntry.Metadata.GetTableName(),
                            Date = DateTime.Now.ToUniversalTime(),
                            User = _currentUser.Name,
                            NewValues = JsonSerializer.Serialize(
                                entityEntry.Properties.Where(
                                    p => !p.Metadata.IsPrimaryKey() &&
                                         !auditExcludedProps.Contains(p.Metadata.Name)).ToDictionary(
                                    p => p.Metadata.Name,
                                    p => p.CurrentValue))
                        }
                    ));
            }

            return returnList;
        }

        private async Task OnAfterSaveChanges(
            IEnumerable<(EntityEntry EntityEntry, Audit Audit)> auditEntries)
        {
            if (!_auditSettings.Enabled || _auditSettings.Sink != AuditSink.Database)
                return;

            if (auditEntries != null && auditEntries.Any())
            {
                foreach (var auditEntry in auditEntries)
                {
                    foreach (var prop in auditEntry.EntityEntry.Properties.Where(x => x.Metadata.IsPrimaryKey()))
                    {
                        auditEntry.Audit.KeyValues = JsonSerializer.Serialize(prop.CurrentValue);
                    }

                    Audits.Add(auditEntry.Audit);
                }

                await SaveChangesAsync();
            }

            await Task.CompletedTask;
        }

        private void UpdateAuditFieldsOnEntities()
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
                _permissionsInRole = new List<Permissions> {Permissions.ReadUsers}.PackPermissions()
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
                _permissionsInRole = new List<Permissions> {Permissions.EditUsers}.PackPermissions()
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