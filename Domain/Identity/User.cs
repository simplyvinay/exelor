using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Exelor.Infrastructure;
using Exelor.Infrastructure.Auditing;

namespace Exelor.Domain.Identity
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
            byte[] salt)
        {
            FirstName = firstName;
            LastName = lastName;
            FullName = $"{FirstName} {LastName}";
            Email = email;
            UserName = userName;
            PhoneNumber = phoneNumber;
            Hash = hash;
            Salt = salt;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string TwoFactorEnabled { get; set; }
        [JsonIgnore]
        [DoNotAudit]
        public byte[] Hash { get; set; }

        [JsonIgnore]
        [DoNotAudit]
        public byte[] Salt { get; set; }

        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
        public ICollection<UserRole> Roles { get; } = new HashSet<UserRole>();

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