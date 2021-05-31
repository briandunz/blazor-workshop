using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BlazingPizza.Shared.Test
{
    public class PizzaTest
    {
        [Fact]
        public void BasePriceCalculatesCorrectlyForDefaultSize()
        {
            var pizzaSpecial = new PizzaSpecial { BasePrice = 10M };
            var subject = new Pizza { Special = pizzaSpecial, Size = Pizza.DefaultSize };

            Assert.Equal(pizzaSpecial.BasePrice, subject.GetBasePrice());
        }

        [Fact]
        public void BasePriceCalculatesCorrectlyForNonDefaultSize()
        {
            var pizzaSpecial = new PizzaSpecial { BasePrice = 10M };
            var subject = new Pizza { Special = pizzaSpecial, Size = 10, Toppings = new List<PizzaTopping>() };
            var expected = ((decimal) subject.Size / (decimal) Pizza.DefaultSize) * pizzaSpecial.BasePrice;
            Assert.Equal(expected, subject.GetBasePrice());
            Assert.Equal(expected, subject.GetTotalPrice());
        }

        [Fact]
        public void TotalPriceIncludesOptionalToppings()
        {
            var pizzaSpecial = new PizzaSpecial { BasePrice = 10M };
            var topping = new Topping { Price = 2.50M };
            var subject = new Pizza { Special = pizzaSpecial, Size = 10, Toppings = new List<PizzaTopping> { new PizzaTopping { Topping = topping } } };
            var expected = ((decimal)subject.Size / (decimal)Pizza.DefaultSize) * pizzaSpecial.BasePrice + subject.Toppings.Sum(x => x.Topping.Price);
            Assert.Equal(expected, subject.GetTotalPrice());
        }
    }
}
