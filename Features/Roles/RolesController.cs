using System.Collections.Generic;
using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Dto;
using Exelor.Helpers;
using Exelor.Infrastructure.Auth.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Exelor.Features.Roles
{
    [ApiVersion("1")]
    [ApiVersion("2")]
    [Route("api/v{version:ApiVersion}/roles")]
    [HasPermission(Permissions.RoleManager)]
    public class RolesController
    {
        private readonly IMediator _mediator;

        public RolesController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
        public async Task<object> Get(
            ListResourceParameters resourceParams)
        {
            return await _mediator.Send(new RoleList.Query(resourceParams));
        }

        [Route("getroles")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
        [MapToApiVersion("2")] // v2 specific action for GET api/roles endpoint
        public async Task<object> Getv2(
            ListResourceParameters resourceParams)
        {
            return await _mediator.Send(new RoleList.Query(resourceParams));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleDto), 200)]
        public async Task<object> Get(
            SingleResourceParameters resourceParams)
        {
            return await _mediator.Send(new RoleDetails.Query(resourceParams));
        }

        [HttpPost]
        public async Task<RoleDto> Create(
            [FromBody] CreateRole.Command command)
        {
            return await _mediator.Send(command);
        }
        
        [HttpPut]
        public async Task<RoleDto> Update(
            [FromBody] UpdateRole.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task Delete(
            int id)
        {
            await _mediator.Send(new DeleteRole.Command(id));
        }
    }
}