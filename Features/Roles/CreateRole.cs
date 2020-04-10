using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Domain.Identity;
using Exelor.Dto;
using Exelor.Infrastructure.Data;
using Exelor.Infrastructure.ErrorHandling;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Exelor.Features.Roles
{
public class CreateRole
    {
        public class Command : IRequest<RoleDto>
        {
            public Command(
                string name)
            {
                Name = name;
            }

            public string Name { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(200);
            }
        }

        public class Handler : IRequestHandler<Command, RoleDto>
        {
            private readonly ApplicationDbContext _dbContext;

            public Handler(
                ApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<RoleDto> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                if (await _dbContext.Roles.AnyAsync(
                    x => x.Name == request.Name,
                    cancellationToken))
                {
                    throw new HttpException(
                        HttpStatusCode.BadRequest,
                        new
                        {
                            Error = $"There is already a role with name {request.Name}."
                        });
                }

                var role = new Role(
                    request.Name);

                await _dbContext.Roles.AddAsync(
                    role,
                    cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new RoleDto(
                    role.Id,
                    role.Name);
            }
        }
    }
}