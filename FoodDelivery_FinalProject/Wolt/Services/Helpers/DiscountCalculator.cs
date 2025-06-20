using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    public static class DiscountCalculator
    {
        public static decimal ApplyDiscounts(decimal price, IEnumerable<decimal> discounts)
        {
            foreach (var discount in discounts)
            {
                price -= price * (discount / 100);
            }
            return Math.Round(price, 2);
        }
    }
}
