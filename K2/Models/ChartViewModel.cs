using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.Models
{
    public class ChartViewModel
    {
        public List<ChartDataItem> series { get; set; }

        public ChartViewModel()
        {
            this.series = new List<ChartDataItem>();
        }
    }

    public class ChartDataItem
    {
        /// <summary>
        /// Key of the corresponding group for this data item
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// Subtotal of the selected aggregate column
        /// </summary>        
        public double value { get; set; }
    }


}