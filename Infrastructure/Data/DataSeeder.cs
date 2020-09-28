using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Infrastructure.Auth.Authentication;

namespace Exelor.Infrastructure.Data
{
    public class DataSeeder
    {
        internal static async Task SeedData(
            ApplicationDbContext context,
            IPasswordHasher passwordHasher)
        {
            if (!context.Users.Any())
            {

                var salt = Guid.NewGuid().ToByteArray();
                var user1 = new User(
                    "John",
                    "Doe",
                    "john@demo.com",
                    "john",
                    string.Empty,
                    passwordHasher.Hash(
                        "test",
                        salt),
                    salt
                );
                context.Users.Add(user1);

                var role1 = new Role("Base User");
                role1.AddPermissions(new List<Permissions> {Permissions.ReadUsers});
                context.Roles.Add(role1);

                var userRole1 = new UserRole(user1, role1);
                context.UserRoles.Add(userRole1);
                
                var user2 = new User(
                    "Jane",
                    "Doe",
                    "jane@demo.com",
                    "jane",
                    string.Empty,
                    passwordHasher.Hash(
                        "test",
                        salt),
                    salt
                );
                context.Users.Add(user2);

                var role2 = new Role("Super User");
                role2.AddPermissions(new List<Permissions> {Permissions.SuperUser});
                context.Roles.Add(role2);

                var userRole2 = new UserRole(user2, role2);
                context.UserRoles.Add(userRole2);
                
                await context.SaveChangesAsync();
            }
        }
    }
}