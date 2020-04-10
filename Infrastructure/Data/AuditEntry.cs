using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Exelor.Infrastructure.Auditing;
using Exelor.Infrastructure.Auth.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Exelor.Infrastructure.Data
{
    public class AuditEntry
    {
        public AuditEntry(
            EntityEntry entityEntry,
            ICurrentUser currentUser)
        {
            var auditExcludedProps = entityEntry.Entity.GetType()
                .GetProperties()
                .Where(
                    p => p.GetCustomAttributes(
                        typeof(DoNotAudit),
                        false).Any())
                .Select(p => p.Name)
                .ToList();

            Table = entityEntry.Metadata.GetTableName();
            Date = DateTime.Now.ToUniversalTime();
            UserId = currentUser.Id;
            UserName = currentUser.Name;

            foreach (var property in entityEntry.Properties.Where(x => !auditExcludedProps.Contains(x.Metadata.Name)))
            {
                if (property.IsTemporary)
                {
                    TemporaryProperties.Add(property);
                    continue;
                }

                var propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            OldValues[propertyName] = property.OriginalValue;
                            NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }

        public DateTime Date { get; }
        public string UserId { get; }
        public string UserName { get; }
        public string Table { get; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        [JsonIgnore]
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();
        
        public Audit AsAudit()
        {
            return new Audit
            {
                Table = Table,
                Date = Date,
                UserId = UserId,
                UserName = UserName,
                KeyValues = JsonSerializer.Serialize(KeyValues),
                NewValues = JsonSerializer.Serialize(NewValues),
                OldValues = JsonSerializer.Serialize(OldValues)
            };
        }
    }
}