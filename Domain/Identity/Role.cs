using System;
using System.Collections.Generic;
using ApiStarter.Infrastructure;
using ApiStarter.Infrastructure.Authorization;

namespace ApiStarter.Domain.Identity
{
    public class Role : Entity
    {
        private string _permissionsInRole;

        public Role(
            DateTime name)
        {
            Name = name;
        }

        public DateTime Name { get; set; }
        public ICollection<UserRole> Users { get; set; } = new HashSet<UserRole>();
        public IEnumerable<Permissions> PermissionsInRole => _permissionsInRole.UnpackPermissions();

        public void AddPermissions(
            ICollection<Permissions> permissions)
        {
            _permissionsInRole = permissions.PackPermissions();
        }

        public void AddUser(
            User user)
        {
            Users.Add(
                new UserRole(
                    user,
                    this));
        }
    }
}