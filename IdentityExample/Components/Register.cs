using IdentityExample.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Components
{
    [ViewComponent]
    public class Register : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync(string returnUrl = null)
        {
            RegisterViewModel model = new RegisterViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

    }
}
