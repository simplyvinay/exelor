using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Dto;
using Exelor.Helpers;
using Exelor.Helpers.Extensions;
using Exelor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Exelor.Features.Roles
{
    public class RoleList
    {
        public class Query : IRequest<object>
        {
            public ListResourceParameters ResourceParams { get; }

            public Query(
                ListResourceParameters resourceParams)
            {
                ResourceParams = resourceParams;
            }
        }

        public class QueryHandler : IRequestHandler<Query, object>
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

            public async Task<object> Handle(
                Query message,
                CancellationToken cancellationToken)
            {
                var roles = _dbContext.Roles.AsNoTracking();
                var filteredRoles = await _sieveProcessor.Apply(
                    message.ResourceParams,
                    roles).ToListAsync(cancellationToken);
                var roleDtos = filteredRoles.Select(
                    role => new RoleDto(
                        role.Id,
                        role.Name,
                        string.Join(
                            ", ",
                            role.PermissionsInRole)));
                return roleDtos.ShapeData(message.ResourceParams.Fields);
            }
        }
    }
}