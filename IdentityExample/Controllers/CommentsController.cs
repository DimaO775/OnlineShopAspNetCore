using IdentityExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace IdentityExample.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<User> userManager;

        public CommentsController(ShopDbContext context, UserManager<User> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var shopDbContext = _context.Comments.Include(c => c.ParentComment);
            return View(await shopDbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, Rating, Content, Advantages, Limitations, ProductId, ParentCommentId")] Comment comment)
        {
            string dateTime = DateTime.Today.ToString("dd MMMM yyyy");
            User user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            comment.User = user;
            if (comment.ParentCommentId == 0)
                comment.ParentCommentId = null;

            if (comment.Advantages == null)
                comment.Advantages = "Нет";
            if (comment.Limitations == null)
                comment.Limitations = "Нет";
            Product product = await _context.Products.FindAsync(comment.ProductId);
            if (product == null)
                return NotFound();
            comment.Date = dateTime;
            //Comment comment = new Comment { Content = content, ProductId = productId, UserId = user.Id};
            if (comment.Content != null)
            {
                _context.Comments.Add(comment);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("GetComments", "Products", new { id = comment.ProductId});
        }

        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Delete(int? id, string returnUrl)
        {
            Comment comment = _context.Comments.Where(t => t.Id == id).Include(t => t.ChildComments).FirstOrDefault();
            await _context.Entry(comment).Collection(t => t.ChildComments).LoadAsync();
            if (comment.ChildComments.Any())
            {
                _context.RemoveRange(comment.ChildComments);
                await _context.SaveChangesAsync();
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return Redirect(returnUrl);
        }
    }
}
