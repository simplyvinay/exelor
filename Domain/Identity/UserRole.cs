using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiStarter.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace ApiStarter.Domain.Identity
{
    public class UserRole : IdentityUserRole<int>, IEntity
    {
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; }
        public DateTime CreatedAt { get; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; }
        public string UpdatedBy { get; set; }
        public bool Archived { get; }
        public User User { get; set; }
        public Role Role { get; set; }


        public void Archive()
        {
            throw new NotImplementedException();
        }

        public void UnArchive()
        {
            throw new NotImplementedException();
        }
    }
}