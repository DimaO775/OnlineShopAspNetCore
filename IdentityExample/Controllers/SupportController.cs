
using IdentityExample.Models;
using IdentityExample.Services;
using IdentityExample.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class SupportController : Controller
    {

        private readonly ShopDbContext _context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IEmailService emailService;

        public SupportController(ShopDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
        {
            this._context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
        }
/*        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }*/

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            User user = await userManager.GetUserAsync(User);
            ViewData["SupportThemesId"] = new SelectList(new[] { new { Id = 0, Topic = "Выбрать..." } }.Union(_context.SupportThemes.Select(t => new { t.Id, t.Topic })), "Id", "Topic");
            List<Support> supports = await _context.Support.Where(t => t.UserId == user.Id).Include(t => t.ChildMessage).Include(t=>t.User).Include(t => t.SupportThemes).ToListAsync();
            return View(new SupportViewModel { Supports = supports });
        }

        public async Task<IActionResult> AddMessage([Bind("Id,Name,Content,SupportThemesId,ParentMessageId")]Support support, int? topicId, int? parentMessageId, SupportViewModel viewModel)
        {
            User user = await userManager.GetUserAsync(User);
            support.IsResolved = false;
            support.UserId = user.Id;
            support.Date = DateTime.Now.ToString();
            if (topicId != null)
            {
                support.SupportThemesId = topicId.Value;
                _context.Support.Where(t => t.Id == parentMessageId).FirstOrDefault().IsResolved = true;
            }
            support.ParentMessageId = parentMessageId;
            await _context.Support.AddAsync(support);
            await _context.SaveChangesAsync();
            if(User.IsInRole("admin"))
                return RedirectToAction("AdminIndex", "Support");
            return RedirectToAction("Index", "Support");
        }

        public async Task<IActionResult> CloseAnswer(int id)
        {
            Support support = await _context.Support.Where(t => t.Id == id).FirstOrDefaultAsync();
            List<Support> supports = await _context.Support.Where(t => t.ParentMessageId == support.Id).ToListAsync();
            _context.Support.Remove(support);
            _context.Support.RemoveRange(supports);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Support");
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminIndex()
        {
            User user = await userManager.GetUserAsync(User);            
            List<SupportThemes> supportThemes = await _context.SupportThemes.Include(t => t.Supports).ToListAsync();
            List<Support> supports = await _context.Support.Where(t => t.IsResolved == false && t.ParentMessageId == null).Include(t=>t.User).Include(t=>t.ChildMessage).ToListAsync();
            return View(new SupportViewModel { SupportThemes = supportThemes, Supports = supports});
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminShowTopic(int? id)
        {
            SupportThemes currentTopic = new SupportThemes();
            List<SupportThemes> supportThemes = await _context.SupportThemes.Include(t => t.Supports).ToListAsync();
            List<Support> supports = await _context.Support.Where(t => t.IsResolved == false && t.ParentMessageId == null).Include(t => t.User).Include(t => t.ChildMessage).Include(t=>t.SupportThemes) .ToListAsync();

            if (id != null)
            {
                currentTopic = await _context.SupportThemes.Where(t => t.Id == id).Include(t => t.Supports).FirstOrDefaultAsync();
                supports = supports.Where(t => t.SupportThemesId == currentTopic.Id).ToList();
            }
            return View(new SupportViewModel { SupportThemes = supportThemes, Supports = supports, CurrentTopic = currentTopic });
        }
    }
}
