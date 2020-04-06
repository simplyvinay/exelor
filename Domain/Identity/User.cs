using System;
using System.Collections.Generic;
using System.Linq;
using ApiStarter.Infrastructure;
using Newtonsoft.Json;

namespace ApiStarter.Domain.Identity
{
    public class User : Entity
    {

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
        public byte[] Hash { get; set; }

        [JsonIgnore]
        public byte[] Salt { get; set; }

        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

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