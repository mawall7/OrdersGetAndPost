using System;
using System.Collections.Generic;
using System.Text;

namespace GetPostOrders
{
    public class Order
    {
        public string OrderName { get; set; }
        public int OrderNumber { get; set; }
        public bool IsReady { get; set; }
        public bool TakeAway { get; set; }

    }
}
