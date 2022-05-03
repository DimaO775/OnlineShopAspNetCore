using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class PaymentCard
    {
        public int Id { get; set; }
        public long Number { get; set; }
        public string ValidUntil { get; set; }

        [ForeignKey(nameof(Payment))]
        public int PaymentId { get; set; }       
        public Payment Payment { get; set; }
    }
}
