using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace CarProduct.Api
{
    public class ValidationPipe<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipe(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = _validators
                .Select(r => r.Validate(context))
                .SelectMany(r => r.Errors)
                .Where(r => r != null)
                .ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
                //throw new BadHttpRequestException
            }

            return next();
        }
    }
}