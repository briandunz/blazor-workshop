using System.Collections.Generic;
using Xunit;

namespace BlazingPizza.Shared.Test
{
    public class ToppingTest
    {
        [Fact]
        public void PriceFormatterReturnsFormattedString()
        {
            var subjectPrice = 1.00M;
            var subject = new Topping { Price = subjectPrice };

            Assert.Equal(subjectPrice.ToString("0.00"), subject.GetFormattedPrice());
        }
    }
}
