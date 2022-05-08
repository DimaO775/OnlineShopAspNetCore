using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class FavoritesProducts
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public DateTime Date { get; set; }

        public Product Product{ get; set; }
        public User User{ get; set; }
    }
}
