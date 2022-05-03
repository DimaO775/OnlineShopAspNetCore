using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public long NumberOfPhone { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int Price { get; set; }

        [ForeignKey(nameof(DeliveryStatus))]
        public int? DeliveryStatusId { get; set; }
        public bool? IsReceived { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }
        public Payment Payment { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
