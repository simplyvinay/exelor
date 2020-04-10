using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Exelor.Infrastructure.Auditing;

namespace Exelor.Infrastructure
{
    public interface IEntity
    {
        int Id { get; }
        public DateTime CreatedAt { get; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; }
        public string UpdatedBy { get; set; }
        bool Archived { get; }
        void Archive();
        void UnArchive();
    }
    
    public class Entity : IEntity
    {
        protected Entity() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DoNotAudit]
        public DateTime CreatedAt { get; set; }
        [DoNotAudit]
        public string CreatedBy { get; set; }
        [DoNotAudit]
        public DateTime UpdatedAt { get; set; }
        [DoNotAudit]
        public string UpdatedBy { get; set; }

        public bool Archived { get; set; }
        
        public void Archive()
        {
            Archived = true;
        }

        public void UnArchive()
        {
            Archived = false;
        }
    }
}