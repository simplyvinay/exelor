using System.Collections.Generic;
using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Dto;
using Exelor.Infrastructure.Auth.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Exelor.Features.Roles
{
    [Route("api/roles")]
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
        public async Task<List<RoleDto>> Get(
            int pageNumber,
            int pageSize,
            string sortBy,
            string sortDirection,
            string filterParams)
        {
            return await _mediator.Send(
                new RoleList.Query(
                    pageNumber,
                    pageSize,
                    sortBy,
                    sortDirection,
                    filterParams));
        }

        [HttpGet("{id}")]
        public async Task<RoleDto> Get(
            int id)
        {
            return await _mediator.Send(new RoleDetails.Query(id));
        }

        [HttpPost]
        public async Task<RoleDto> Create(
            [FromBody] CreateRole.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("{id}")]
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