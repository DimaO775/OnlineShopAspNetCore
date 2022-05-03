using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class DeliveryStatus
    {
        public int Id { get; set; }
        public string Status { get; set; }

        public List<Order> Orders { get; set; }
    }
}
