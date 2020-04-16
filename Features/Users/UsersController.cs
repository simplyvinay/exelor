using System.Net;
using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Dto;
using Exelor.Helpers;
using Exelor.Helpers.Extensions;
using Exelor.Infrastructure.Auth.Authentication;
using Exelor.Infrastructure.Auth.Authorization;
using Exelor.Infrastructure.ErrorHandling;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Exelor.Features.Users
{
    [Route("api/users")]
    public class UsersController
    {
        private readonly ICurrentUser _currentUser;
        private readonly IMediator _mediator;

        public UsersController(
            ICurrentUser currentUser,
            IMediator mediator)
        {
            _currentUser = currentUser;
            _mediator = mediator;
        }

        [HttpGet]
        [HasPermission(
            Permissions.ReadUsers,
            Permissions.EditUsers)]
        [ProducesResponseType(typeof(UserDetailsDto), 200)]
        public async Task<object> Get(
            ListResourceParameters resourceParams)
        {
            return await _mediator.Send(new UserList.Query(resourceParams));
        }

        [HttpPut]
        public async Task<UserDetailsDto> Edit(
            [FromBody] UpdateUser.Command command)
        {
            if (!_currentUser.IsAllowed(Permissions.EditUsers))
                throw new HttpException(HttpStatusCode.Forbidden);

            return await _mediator.Send(command);
        }


        [HttpDelete("{id}")]
        public async Task Delete(
            int id)
        {
            _currentUser.Authorize(Permissions.EditUsers);
            await _mediator.Send(new DeleteUser.Command(id));
        }
    }
}