using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Auth.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exelor.Features.Users
{
    [Route("api/users")]
    public class UsersController
    {
        private readonly IMediator _mediator;
        private ICurrentUser _currentUser;

        public UsersController(
            IMediator mediator,
            ICurrentUser currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<UserDto> Login(
            [FromBody] Login.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpGet]
        [HasPermission(Permissions.ReadUsers)]
        public string Get()
        {
            return "User Listing";
        }
        
        [HttpPost]
        [HasPermission(Permissions.EditUsers)]
        public string Edit()
        {
            return "Edit Users";
        }
    }
}