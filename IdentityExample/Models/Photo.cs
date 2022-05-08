using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsMain { get; set; }
        public bool IsSlider { get; set; }

        [ForeignKey(nameof(Product))]
        public int? ProductId { get; set; }
        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }

        public Category Category { get; set; }
        public Product Product { get; set; }
    }
}
