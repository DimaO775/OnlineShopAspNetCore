using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModels
{
    public class AddCommentViewModels
    {
        [Required]
        [Display(Name = "Отзыв")]
        public string Content { get; set; }
    }
}
