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
        private readonly ICurrentUser _currentUser;

        public UsersController(
            IMediator mediator,
            ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet]
        [HasPermission(Permissions.ReadUsers, Permissions.EditUsers)]
        public string Get()
        {
            _ = _currentUser.Id;
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