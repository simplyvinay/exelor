using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Dto;
using Exelor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Exelor.Features.Users
{
    public class UserList
    {
        public class Query : IRequest<List<UserDetailsDto>>
        {
        }

        public class QueryHandler : IRequestHandler<Query, List<UserDetailsDto>>
        {
            private readonly ApplicationDbContext _dbContext;

            public QueryHandler(
                ApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            //Implement Paging
            public async Task<List<UserDetailsDto>> Handle(
                Query message,
                CancellationToken cancellationToken)
            {
                return await _dbContext.Users.AsNoTracking()
                    .Include(x => x.Roles)
                    .ThenInclude(x => x.Role)
                    .Select(
                    user => new UserDetailsDto(
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.FullName,
                        user.Email,
                        string.Join(", ", user.Roles.Select(r => r.Role.Name)))
                    ).ToListAsync(cancellationToken);
            }
        }
    }
}