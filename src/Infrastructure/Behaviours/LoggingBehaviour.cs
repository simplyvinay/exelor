using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Behaviours
{
    public class LoggingBehaviour<TRequest> 
        : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;
        private readonly ICurrentUser _currentUser;

        public LoggingBehaviour(
            ILogger<TRequest> logger,
            ICurrentUser currentUser)
        {
            _logger = logger;
            _currentUser = currentUser;
        }


        public Task Process(
            TRequest request,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogInformation(
                "Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName,
                _currentUser.Id,
                _currentUser.Name,
                request);
            return Unit.Task;
        }
    }
}