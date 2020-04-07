using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Data;
using Exelor.Infrastructure.ErrorHandling;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Exelor.Features.Users
{
    public class Login
    {
        public class Command : IRequest<UserDto>
        {
            private Command() { }

            public Command(
                string userName,
                string password)
            {
                UserName = userName;
                Password = password;
            }

            public string UserName { get; set; }
            public string Password { get; set; }

        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.UserName).NotNull().NotEmpty();
                RuleFor(x => x.Password).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, UserDto>
        {
            private readonly IPasswordHasher _passwordHasher;
            private readonly IJwtTokenGenerator _jwtTokenGenerator;
            private readonly ApplicationDbContext _context;

            public Handler(
                IPasswordHasher passwordHasher,
                IJwtTokenGenerator jwtTokenGenerator,
                ApplicationDbContext context)
            {
                _passwordHasher = passwordHasher;
                _jwtTokenGenerator = jwtTokenGenerator;
                _context = context;
            }

            public async Task<UserDto> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                var user = await _context.Users.SingleOrDefaultAsync(
                    x => x.UserName == request.UserName && !x.Archived,
                    cancellationToken);

                if (user == null)
                {
                    throw new HttpException(
                        HttpStatusCode.Unauthorized,
                        new {Error = "Invalid credentials."});
                }

                if (!user.Hash.SequenceEqual(
                    _passwordHasher.Hash(
                        request.Password,
                        user.Salt)))
                {
                    throw new HttpException(
                        HttpStatusCode.Unauthorized,
                        new {Error = "Invalid credentials."});
                }

                // generate refresh token
                var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
                user.AddRefreshToken(
                    refreshToken,
                    user.Id);

                var roles = _context.UserRoles
                    .Include(x => x.Role)
                    .Where(x => x.User == user);

                var permissions = new List<Permissions>();
                foreach (var userRole in roles)
                {
                    permissions.AddRange(userRole.Role.PermissionsInRole.ToList());
                }

                var token = await _jwtTokenGenerator.CreateToken(
                    user.Id.ToString(),
                    permissions);
                await _context.SaveChangesAsync(cancellationToken);

                return new UserDto(
                    user.FirstName,
                    user.LastName,
                    user.FullName,
                    user.Email,
                    token,
                    refreshToken);
            }
        }
    }
}