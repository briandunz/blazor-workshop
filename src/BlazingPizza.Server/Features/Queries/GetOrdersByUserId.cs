using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace BlazingPizza.Server.Features.Queries
{
    public class GetOrdersByUserId
    {
        public class Query: IRequest<Result>
        {
            public string UserId { get; set; }
        }

        // server side validation
        public class QueryValidator: AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.UserId).NotEmpty();
            }
        }

        public class Result
        {
            // fill in order details
        }

        public class GetOrdersByUserIdHandler : IRequestHandler<Query, Result>
        {
            public GetOrdersByUserIdHandler()
            {
                // retrieve the orders
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                await Task.Delay(1); // stubbed out
                return new Result();
            }
        }
    }
}
