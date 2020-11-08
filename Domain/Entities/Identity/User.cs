using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities.Identity
{
    public class User : Entity
    {
        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();

        public User(
            string firstName,
            string lastName,
            string email,
            string userName,
            string phoneNumber,
            byte[] hash,
            byte[] salt,
            Address address)
        {
            FirstName = firstName;
            LastName = lastName;
            FullName = $"{FirstName} {LastName}";
            Email = email;
            UserName = userName;
            PhoneNumber = phoneNumber;
            Hash = hash;
            Salt = salt;
            Address = address;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName { get; }
        public string Email { get; }
        public string UserName { get; }
        public string PhoneNumber { get; }
        public string TwoFactorEnabled { get; }
        [JsonIgnore]
        [DoNotAudit]
        public byte[] Hash { get; }

        [JsonIgnore]
        [DoNotAudit]
        public byte[] Salt { get; }

        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
        public ICollection<UserRole> Roles { get; } = new HashSet<UserRole>();
        public Address Address { get; set; }

        public void Update(
            string firstName,
            string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public void AddRefreshToken(
            string token,
            double daysToExpire = 2)
        {
            _refreshTokens.Add(
                new RefreshToken(
                    token,
                    DateTime.UtcNow.AddDays(daysToExpire),
                    this));
        }

        public void RemoveRefreshToken(
            string refreshToken)
        {
            _refreshTokens.Remove(_refreshTokens.First(t => t.Token == refreshToken));
        }

        public bool IsValidRefreshToken(
            string refreshToken)
        {
            return _refreshTokens.Any(rt => rt.Token == refreshToken && rt.Active);
        }
    }
}