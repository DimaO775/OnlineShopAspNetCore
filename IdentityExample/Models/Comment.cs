using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; }
        public string Date { get; set; }



        [ForeignKey(nameof(ParentComment))]
        public int? ParentCommentId { get; set; }



        public Comment ParentComment { get; set; }
        public List<Comment> ChildComments { get; set; }



        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public Product Product { get; set; }
        public User User { get; set; }
    }
}
