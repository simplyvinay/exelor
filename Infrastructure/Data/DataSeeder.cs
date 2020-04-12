using System;
using System.Collections.Generic;
using Exelor.Domain.Identity;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Auth.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Exelor.Infrastructure.Data
{
    public class DataSeeder
    {
        internal static void SeedData(
            ModelBuilder builder,
            IPasswordHasher passwordHasher)
        {
            var createdAt = DateTime.Now;
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
            )
            {
                Id = 1,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                Archived = false
            };

            var role1 = new
            {
                Id = 1,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                Archived = false,
                Name = "Base User",
                _permissionsInRole = new List<Permissions> {Permissions.ReadUsers}.PackPermissions()
            };

            builder.Entity<UserRole>().HasData(
                new
                {
                    UserId = 1,
                    RoleId = 1
                }
            );

            builder.Entity<User>().HasData(user1);
            builder.Entity<Role>().HasData(role1);

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
            )
            {
                Id = 2,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                Archived = false
            };

            var role2 = new
            {
                Id = 2,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                Archived = false,
                Name = "Base+ User",
                _permissionsInRole = new List<Permissions> {Permissions.SuperUser}.PackPermissions()
            };

            builder.Entity<UserRole>().HasData(
                new
                {
                    UserId = 2,
                    RoleId = 2
                }
            );

            builder.Entity<User>().HasData(user2);
            builder.Entity<Role>().HasData(role2);
        }
    }
}