using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Server.Features.Queries
{
    public class GetOrder
    {
        public class Query : IRequest<Result>
        {
            public int? OrderId { get; set; }
            public string UserId { get; set; }
        }

        // server side validation
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.OrderId).NotEmpty();
                RuleFor(x => x.UserId).NotEmpty();
            }
        }

        public class Result
        {
            public OrderWithStatus Order { get; set; }
        }

        public class GetOrdersHandler : IRequestHandler<Query, Result>
        {
            private readonly PizzaStoreContext _db;

            public GetOrdersHandler(PizzaStoreContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var order = await _db.Orders
                .Where(o => o.OrderId == request.OrderId)
                .Where(o => o.UserId == request.UserId)
                .Include(o => o.DeliveryLocation)
                .Include(o => o.Pizzas).ThenInclude(p => p.Special)
                .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
                .SingleOrDefaultAsync();


                return new Result
                {
                    Order = order == null ? null : OrderWithStatus.FromOrder(order)
                };
            }
        }
    }
}
