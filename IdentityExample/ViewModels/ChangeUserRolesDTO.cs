using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModels
{
    public class ChangeUserRolesDTO
    {
        public string Id { get; set; }

        public string Login { get; set; }
        public string Email { get; set; }

        public List<IdentityRole> AllRoles { get; set; }

        public IList<string> UserRoles { get; set; }
    }
}
