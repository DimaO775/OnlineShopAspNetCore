using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class Support
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public bool IsResolved { get; set; }
        
        [ForeignKey(nameof(SupportThemes))]
        public int SupportThemesId { get; set; }

        [ForeignKey(nameof(ParentMessage))]
        public int? ParentMessageId { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public User User { get; set; }
        public Support ParentMessage { get; set; }
        public List<Support> ChildMessage { get; set; }
        public SupportThemes SupportThemes { get; set; }
    }
}
