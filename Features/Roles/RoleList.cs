using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Dto;
using Exelor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Exelor.Infrastructure.Data.DataProcessor;
using Exelor.Infrastructure.Data.DataProcessor.Mapping;

namespace Exelor.Features.Roles
{
    public class RoleList
    {
        public class Query : QueryBase<Role>, IRequest<List<RoleDto>>
        {
            public Query(
                int pageNumber,
                int pageSize,
                string sortBy,
                string sortDirection,
                string filterParams) : base(
                pageNumber,
                pageSize,
                sortBy,
                sortDirection,
                filterParams)
            {
            }

            public override MappingCollection<Role> GetMappings()
            {
                var maps = new List<IFilterMap<Role>>
                {
                    new FilterMapping<Role, int>(
                        "id",
                        et => et.Id) {IsDefaultSortFilter = true},
                    new FilterMapping<Role, string>(
                        "name",
                        et => et.Name),
                };

                return new MappingCollection<Role>(maps);
            }
        }

        public class QueryHandler : IRequestHandler<Query, List<RoleDto>>
        {
            private readonly ApplicationDbContext _dbContext;

            public QueryHandler(
                ApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public Task<List<RoleDto>> Handle(
                Query message,
                CancellationToken cancellationToken)
            {
                var roles = _dbContext.Roles.AsNoTracking().Apply(
                    message.FilterSet,
                    message.GetMappings()).Results;

                return Task.Run(
                    () => roles.Select(
                        x => new RoleDto(
                            x.Id,
                            x.Name)).ToList(),
                    cancellationToken);
            }
        }
    }
}