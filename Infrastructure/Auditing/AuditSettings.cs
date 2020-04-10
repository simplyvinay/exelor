using System.Collections.Generic;

namespace Exelor.Infrastructure.Auditing
{
    public enum AuditSink
    {
        Database,
        Log
    }

    public class AuditSettings
    {
        public bool Enabled { get; set; }
        public List<AuditSink> Sinks { get; set; }
    }
}