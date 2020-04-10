using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
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
            IOptionsSnapshot<AuditSettings> auditSettings)
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

            builder.Entity<User>()
                .HasMany(a => a.Roles)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

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

            DataSeeder.SeedData(
                builder,
                _passwordHasher);
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

        private async Task<IEnumerable<AuditEntry>> OnBeforeSaveChanges()
        {
            if (!_auditSettings.Enabled)
                return null;

            ChangeTracker.DetectChanges();
            var entitiesToTrack = ChangeTracker.Entries().Where(
                e => !(e.Entity is Audit) && e.State != EntityState.Detached && e.State != EntityState.Unchanged);

            var auditEntries = entitiesToTrack.Select(
                    entityEntry => new AuditEntry(
                        entityEntry,
                        _currentUser))
                .ToList();

            await LogAsync(auditEntries);

            return auditEntries.Where(x => x.TemporaryProperties.Any());
        }

        private async Task OnAfterSaveChanges(
            IEnumerable<AuditEntry> auditEntries)
        {
            if (!_auditSettings.Enabled)
                return;

            if (auditEntries != null && auditEntries.Any())
            {
                foreach (var auditEntry in auditEntries)
                {
                    foreach (var prop in auditEntry.TemporaryProperties)
                    {
                        if (prop.Metadata.IsPrimaryKey())
                        {
                            auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                        }
                        else
                        {
                            auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                        }
                    }
                }

                await LogAsync(auditEntries, true);
            }

            await Task.CompletedTask;
        }

        private async Task LogAsync(
            IEnumerable<AuditEntry> auditEntries, bool saveChanges = false)
        {
            foreach (var auditSink in _auditSettings.Sinks)
            {
                switch (auditSink)
                {
                    case AuditSink.Database:
                        await Audits.AddRangeAsync(auditEntries.Select(auditEntry => auditEntry.AsAudit()).ToList());
                        if (saveChanges)
                        {
                            await SaveChangesAsync();
                        }
                        break;
                    case AuditSink.Log:
                        foreach (var auditEntry in auditEntries)
                        {
                            Log.Information(
                                "{Table} Audit Log {Details}",
                                auditEntry.Table,
                                JsonSerializer.Serialize(auditEntry));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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
    }
}