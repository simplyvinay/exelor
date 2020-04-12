using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Dto;
using Exelor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Exelor.Features.Roles
{
    public class UserList
    {
        public class Query : IRequest<List<RoleDto>>
        {
        }

        public class QueryHandler : IRequestHandler<Query, List<RoleDto>>
        {
            private readonly ApplicationDbContext _dbContext;

            public QueryHandler(
                ApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            //Implement Paging
            public async Task<List<RoleDto>> Handle(
                Query message,
                CancellationToken cancellationToken)
            {
                var roles = await _dbContext.Roles.AsNoTracking().ToListAsync(cancellationToken);
                return roles
                    .Select(x => new RoleDto(x.Id, x.Name)).ToList();
            }
        }
    }
}