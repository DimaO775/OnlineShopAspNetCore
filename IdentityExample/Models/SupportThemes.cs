using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class SupportThemes
    {
        public int Id { get; set; }
        public string Topic { get; set; }

        public List<Support> Supports { get; set; }
    }
}
