using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace StoneShop_Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; }

        public IEnumerable<OrderDetail> OrderDetail { get; set; }

    }
}
    