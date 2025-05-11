using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlakyTestWebApp.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}