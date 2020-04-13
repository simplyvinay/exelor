using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Dto;
using Exelor.Infrastructure.Data;
using Exelor.Infrastructure.ErrorHandling;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Exelor.Features.Roles
{
    public class UpdateRole
    {
        public class Command : IRequest<RoleDto>
        {
            public Command(
                int id,
                string name)
            {
                Id = id;
                Name = name;
            }

            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotNull().NotEmpty();
                RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(50);
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
                    x => x.Name == request.Name && x.Id != request.Id,
                    cancellationToken))
                {
                    throw new HttpException(
                        HttpStatusCode.BadRequest,
                        new
                        {
                            Error = $"There is already a role with name {request.Name}."
                        });
                }

                var role = await _dbContext.Roles.FirstAsync(
                    x => x.Id == request.Id,
                    cancellationToken);
                role.Name = request.Name;

                await _dbContext.SaveChangesAsync(cancellationToken);

                return new RoleDto(
                    role.Id,
                    role.Name);
            }
        }
    }
}