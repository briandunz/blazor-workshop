using System;
using System.Collections.Generic;
using BlazingPizza;
using Xunit;

namespace BlazingPizza.Shared.Test
{
    public class OrderTest
    {
        [Fact]
        public void OrderTotalsPriceForSinglePizzaNoToppings()
        {
            var pizzaSpecial = new PizzaSpecial
            {
                BasePrice = 11.50M
            };
            var pizza = new Pizza
            {
                Special = pizzaSpecial,
                Size = 12,
                Toppings = new List<PizzaTopping>()
            };
            var order = new Order
            {
                Pizzas = new List<Pizza> { pizza }
            };

            Assert.Equal(pizzaSpecial.BasePrice, order.GetTotalPrice());
        }

        [Fact]
        public void OrderTotalsPriceForMultiplePizzaNoToppings()
        {
            var pizzaSpecial1 = new PizzaSpecial
            {
                BasePrice = 11.50M
            };
            var pizzaSpecial2 = new PizzaSpecial
            {
                BasePrice = 12.50M
            };
            var pizza1 = new Pizza
            {
                Special = pizzaSpecial1,
                Size = 12,
                Toppings = new List<PizzaTopping>()
            };
            var pizza2 = new Pizza
            {
                Special = pizzaSpecial2,
                Size = 12,
                Toppings = new List<PizzaTopping>()
            };
            var order = new Order
            {
                Pizzas = new List<Pizza> { pizza1, pizza2 }
            };

            Assert.Equal(pizzaSpecial1.BasePrice + pizzaSpecial2.BasePrice, order.GetTotalPrice());
        }

        [Fact]
        public void TotalPriceIsFormattedCorrectly()
        {
            var pizzaSpecial = new PizzaSpecial
            {
                BasePrice = 11.50M
            };
            var pizza = new Pizza
            {
                Special = pizzaSpecial,
                Size = 12,
                Toppings = new List<PizzaTopping>()
            };
            var order = new Order
            {
                Pizzas = new List<Pizza> { pizza }
            };

            Assert.Equal(pizzaSpecial.BasePrice.ToString("0.00"), order.GetFormattedTotalPrice());
        }

    }
}
