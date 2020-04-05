using System;
using ApiStarter.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace ApiStarter.Domain.Identity
{
    public class UserClaim : IdentityUserClaim<int>, IArchivableEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
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