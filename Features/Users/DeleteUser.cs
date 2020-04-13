using System;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Exelor.Features.Users
{
    public class DeleteUser
    {
        public class Command : IRequest
        {
            private Command() {}

            public Command(
                int id)
            {
                Id = id;
            }

            public int Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly ApplicationDbContext _dbContext;

            public Handler(
                ApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Unit> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                var user = await _dbContext.Users.FirstAsync(
                    x => x.Id == request.Id,
                    cancellationToken);

                if (user == null)
                {
                    throw new Exception("Not Found");
                }

                user.Archive();
                await _dbContext.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}