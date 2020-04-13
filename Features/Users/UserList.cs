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

namespace Exelor.Features.Users
{
    public class UserList
    {
        public class Query : IRequest<List<UserDetailsDto>>
        {
            public SieveModel SieveModel { get; }

            public Query(
                SieveModel sieveModel)
            {
                SieveModel = sieveModel;
            }
        }

        public class QueryHandler : IRequestHandler<Query, List<UserDetailsDto>>
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

            //Implement Paging
            public async Task<List<UserDetailsDto>> Handle(
                Query message,
                CancellationToken cancellationToken)
            {
                var users = _dbContext.Users.AsNoTracking()
                    .Include(x => x.Roles)
                    .ThenInclude(x => x.Role);

                //var count = _sieveProcessor.Apply(message.SieveModel, users, applyPagination: false).Count();

                var filteredUsers = await _sieveProcessor.Apply(
                    message.SieveModel,
                    users).ToListAsync(cancellationToken);

                return filteredUsers
                    .Select(
                    user => new UserDetailsDto(
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.FullName,
                        user.Email,
                        string.Join(", ", user.Roles.Select(r => r.Role.Name)))
                    ).ToList();
            }
        }
    }
}