using IdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModels
{
    public class SupportViewModel
    {
        public Support Support { get; set; }
        public List<Support> Supports { get; set; }
        public User User { get; set; }

        public List<SupportThemes> SupportThemes { get; set; }

        public SupportThemes CurrentTopic { get; set; }
    }
}
