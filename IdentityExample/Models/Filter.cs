using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class Filter
    {
        public List<Manufacturer> Manufacturers{ get; set; }
        public int StartPrice { get; set; }
        public int EndPrice { get; set; }
    }
}
