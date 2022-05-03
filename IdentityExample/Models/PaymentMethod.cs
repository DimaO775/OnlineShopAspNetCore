using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
