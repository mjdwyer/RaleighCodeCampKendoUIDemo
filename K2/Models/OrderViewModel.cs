using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.Models
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public Nullable<decimal> Freight { get; set; }
        public string ShipName { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string ShipCity { get; set; }

    }
}