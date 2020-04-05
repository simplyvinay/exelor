using System;
using ApiStarter.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace ApiStarter.Domain.Identity
{
    public class User : IdentityUser<int>, IEntity
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
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