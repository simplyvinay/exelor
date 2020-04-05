using System;
using System.Collections.Generic;
using ApiStarter.Infrastructure;
using ApiStarter.Infrastructure.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ApiStarter.Domain.Identity
{
    public class Role : IdentityRole<int>, IEntity
    {
        private string _permissionsInRole;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool Archived { get; set; }
        public IEnumerable<Permissions> PermissionsInRole => _permissionsInRole.UnpackPermissions();

        public void AddPermissions(ICollection<Permissions> permissions)
        {
            _permissionsInRole = permissions.PackPermissions();
        }

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