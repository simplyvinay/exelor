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
        public AuditSink Sink { get; set; }
    }
}