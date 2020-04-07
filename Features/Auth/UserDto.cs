using System.Text.Json.Serialization;

namespace Exelor.Features.Auth
{
    public class UserDto
    {
        protected UserDto()
        {
        }

        public UserDto(
            string firstName,
            string lastName,
            string fullName,
            string email,
            string token,
            string refreshToken)
        {
            FirstName = firstName;
            LastName = lastName;
            FullName = fullName;
            Email = email;
            Token = token;
            RefreshToken = refreshToken;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        [JsonIgnore]
        public byte[] Hash { get; set; }

        [JsonIgnore]
        public byte[] Salt { get; set; }
    }
}