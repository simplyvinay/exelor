using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiStarter.Domain.Identity;
using ApiStarter.Infrastructure.Data;
using ApiStarter.Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiStarter.Infrastructure.Security
{
    public interface IAuthenticationService
    {
        Task<string> SignInAsync(string userName, string password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private IJwtTokenGenerator _jwtTokenGenerator;

        public AuthenticationService(
            SignInManager<User> signInManager,
            ApplicationDbContext applicationDbContext,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> SignInAsync(
            string userName,
            string password)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(
                userName,
                password,
                false,
                false);

            if (!signInResult.Succeeded)
            {
                throw new HttpException(
                    HttpStatusCode.Unauthorized,
                    new {Error = "Invalid credentials."});
            }

            var user = _applicationDbContext.Users.AsNoTracking().SingleAsync(x => x.UserName == userName);

            var roles = from role in _applicationDbContext.Roles.AsNoTracking()
                from userRoles in role.Users
                where userRoles.UserId == user.Id
                select role;


            var token = await _jwtTokenGenerator.CreateToken(
                user.Id.ToString(),
                roles.SelectMany(x => x.PermissionsInRole));

            return token;
        }
    }
}