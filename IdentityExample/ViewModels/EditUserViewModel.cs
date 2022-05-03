using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public int Year { get; set; }
    }
}
