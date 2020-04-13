using System.Threading;
using System.Threading.Tasks;
using Exelor.Dto;
using Exelor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Exelor.Features.Roles
{
    public class RoleDetails
    {
        public class Query : IRequest<RoleDto>
        {
            public int Id { get; }

            public Query(
                int id)
            {
                Id = id;
            }
        }

        public class QueryHandler : IRequestHandler<Query, RoleDto>
        {
            private readonly ApplicationDbContext _dbContext;

            public QueryHandler(
                ApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<RoleDto> Handle(
                Query message,
                CancellationToken cancellationToken)
            {
                var role = await _dbContext.Roles.AsNoTracking()
                    .SingleAsync(
                        x => x.Id == message.Id,
                        cancellationToken);
                return new RoleDto(
                    role.Id,
                    role.Name,
                    string.Join(
                        ", ",
                        role.PermissionsInRole));
            }
        }
    }
}