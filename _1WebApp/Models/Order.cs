using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlakyTestWebApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string Product { get; set; }
        public double Amount { get; set; }
        public string Date { get; set; }
        public virtual Customer Customer { get; set; }
    }
}