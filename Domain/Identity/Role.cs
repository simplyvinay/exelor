using System;
using ApiStarter.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace ApiStarter.Domain.Identity
{
    public class Role : IdentityRole<int>, IEntity
    {
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

    public class UserRole : IdentityUserRole<int> { }
    public class UserLogin : IdentityUserLogin<int> { }
    public class RoleClaim : IdentityRoleClaim<int> { }
    public class UserToken : IdentityUserToken<int> { }
}