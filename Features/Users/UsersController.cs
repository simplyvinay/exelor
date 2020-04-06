using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Exelor.Features.Users
{
    [Route("api/users")]
    public class UsersController
    {
        private readonly IMediator _mediator;

        public UsersController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<UserDto> Login(
            [FromBody] Login.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}