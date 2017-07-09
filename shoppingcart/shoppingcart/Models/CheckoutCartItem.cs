using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace shoppingcart.Models
{
    public class CheckoutCartItem
    {
        public int product_id { get; set; }
        public int quantity { get; set; }
    }
}