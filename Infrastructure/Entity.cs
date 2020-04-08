using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public string CreatedBy { get; set; }
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
        [JsonIgnore]
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