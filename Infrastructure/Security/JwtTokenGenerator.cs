using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ApiStarter.Domain.Identity;
using ApiStarter.Infrastructure.Authorization;
using Microsoft.Extensions.Options;

namespace ApiStarter.Infrastructure.Security
{
    public interface IJwtTokenGenerator
    {
        Task<string> CreateToken(
            string userId,
            IEnumerable<Permissions> permissions);

        string GenerateRefreshToken(
            int size = 32);
    }

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtTokenGenerator(
            IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<string> CreateToken(
            string userId,
            IEnumerable<Permissions> permissions)
        {
            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    userId),
                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    await _jwtOptions.JtiGenerator()),
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(_jwtOptions.IssuedAt).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
                new Claim(
                    PermissionClaimName.Permissions,
                    permissions.PackPermissions()),
            };
            var jwt = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                _jwtOptions.NotBefore,
                _jwtOptions.Expiration,
                _jwtOptions.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public string GenerateRefreshToken(
            int size = 32)
        {
            var randomNumber = new byte[size];
            using var numberGenerator = RandomNumberGenerator.Create();
            numberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
