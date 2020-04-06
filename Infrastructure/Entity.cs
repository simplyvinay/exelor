using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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

        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
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