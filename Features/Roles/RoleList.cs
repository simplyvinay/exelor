using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Dto;
using Exelor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Exelor.Features.Roles
{
    public class UserList
    {
        public class Query : IRequest<List<RoleDto>>
        {
            public SieveModel SieveModel { get; }

            public Query(
                SieveModel sieveModel)
            {
                SieveModel = sieveModel;
            }
        }

        public class QueryHandler : IRequestHandler<Query, List<RoleDto>>
        {
            private readonly ApplicationDbContext _dbContext;
            private readonly ISieveProcessor _sieveProcessor;

            public QueryHandler(
                ApplicationDbContext dbContext,
                ISieveProcessor sieveProcessor)
            {
                _dbContext = dbContext;
                _sieveProcessor = sieveProcessor;
            }

            public async Task<List<RoleDto>> Handle(
                Query message,
                CancellationToken cancellationToken)
            {
                var roles = _dbContext.Roles.AsNoTracking();
                var filteredRoles = await _sieveProcessor.Apply(
                    message.SieveModel,
                    roles).ToListAsync(cancellationToken);
                return filteredRoles.Select(
                    role => new RoleDto(
                        role.Id,
                        role.Name,
                        string.Join(
                            ", ",
                            role.PermissionsInRole))).ToList();
            }
        }
    }
}