using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class Discounts
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Value { get; set; }
        public bool IsActive { get; set; }

        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<Manufacturer> Manufacturers { get; set; }
    }
}
