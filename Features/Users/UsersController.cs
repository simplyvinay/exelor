using System.Net;
using Exelor.Domain.Identity;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Auth.Authorization;
using Exelor.Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Mvc;

namespace Exelor.Features.Users
{
    [Route("api/users")]
    public class UsersController
    {
        private readonly ICurrentUser _currentUser;

        public UsersController(
            ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet]
        [HasPermission(Permissions.ReadUsers, Permissions.EditUsers)]
        public string Get()
        {
            return "User Listing";
        }
        
        [HttpPost]
        public string Edit()
        {
            if(!_currentUser.IsAllowed(Permissions.EditUsers))
                throw new HttpException(HttpStatusCode.Forbidden);
            
            return "Edit Users";
        }
        
        [HttpDelete]
        public string Delete()
        {
            _currentUser.Authorize(Permissions.EditUsers);
            return "Delete Users";
        }
    }
}