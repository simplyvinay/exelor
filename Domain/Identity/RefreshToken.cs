using System;
using Exelor.Infrastructure;

namespace Exelor.Domain.Identity
{
    public class RefreshToken : Entity
    {
        protected RefreshToken()
        {
            
        }

        public RefreshToken(
            string token,
            DateTime expires,
            User user)
        {
            Token = token;
            Expires = expires;
            User = user;
        }

        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public User User { get; set; }
        public bool Active => DateTime.UtcNow <= Expires;
    }
}