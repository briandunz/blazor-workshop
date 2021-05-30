using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            public IEnumerable<OrderWithStatus> Orders { get; set; }
        }

        public class GetOrdersByUserIdHandler : IRequestHandler<Query, Result>
        {
            private readonly PizzaStoreContext _db;

            public GetOrdersByUserIdHandler(PizzaStoreContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var orders = await _db.Orders
                .Where(o => o.UserId == request.UserId)
                .Include(o => o.DeliveryLocation)
                .Include(o => o.Pizzas).ThenInclude(p => p.Special)
                .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
                .OrderByDescending(o => o.CreatedTime)
                .ToListAsync();

                return new Result
                {
                    Orders = orders.Select(o => OrderWithStatus.FromOrder(o)).ToList()
                };
            }
        }
    }
}
