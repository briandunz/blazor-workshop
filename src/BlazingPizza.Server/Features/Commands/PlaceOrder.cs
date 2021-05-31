using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace BlazingPizza.Server.Features.Commands
{
    public class PlaceOrder
    {
        public class Command: IRequest<CommandResponse>
        {
            public Order Order { get; set; }
        }

        public class CommandResponse
        {
            public Order SavedOrder { get; set; }
            public string Message { get; set; }
            public bool SuccessfullySaved { get; set; }
        } 

        public class CommandValidator: AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Order.CreatedTime).NotEmpty();
                RuleFor(x => x.Order.DeliveryLocation).NotEmpty();
                RuleFor(x => x.Order.DeliveryAddress).NotEmpty();
                RuleFor(x => x.Order.DeliveryAddress.City).NotEmpty();
                RuleFor(x => x.Order.DeliveryAddress.City).MaximumLength(50);
                RuleFor(x => x.Order.DeliveryAddress.Line1).NotEmpty();
                RuleFor(x => x.Order.DeliveryAddress.Line1).MaximumLength(100);
                RuleFor(x => x.Order.DeliveryAddress.Line2).MaximumLength(100);
                RuleFor(x => x.Order.DeliveryAddress.Name).NotEmpty();
                RuleFor(x => x.Order.DeliveryAddress.Name).MaximumLength(100);
                RuleFor(x => x.Order.DeliveryAddress.Region).NotEmpty();
                RuleFor(x => x.Order.DeliveryAddress.Region).MaximumLength(20);
                RuleFor(x => x.Order.DeliveryAddress.PostalCode).NotEmpty();
                RuleFor(x => x.Order.DeliveryAddress.PostalCode).MaximumLength(20);
            }
        }

        public class CommandHandler: IRequestHandler<Command, CommandResponse>
        {
            private readonly PizzaStoreContext _db;
            public CommandHandler(PizzaStoreContext db)
            {
                _db = db;
            }

            public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                // Enforce existence of Pizza.SpecialId and Topping.ToppingId
                // in the database - prevent the submitter from making up
                // new specials and toppings
                foreach (var pizza in request.Order.Pizzas)
                {
                    pizza.SpecialId = pizza.Special.Id;
                    pizza.Special = null;

                    foreach (var topping in pizza.Toppings)
                    {
                        topping.ToppingId = topping.Topping.Id;
                        topping.Topping = null;
                    }
                }
                _db.Orders.Attach(request.Order);
                await _db.SaveChangesAsync(cancellationToken);
                return new CommandResponse
                {
                    SavedOrder = request.Order,
                    SuccessfullySaved = true
                };
            }
        }
    }
}
