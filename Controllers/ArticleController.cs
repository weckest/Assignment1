using Assignment1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment1.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ArticleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Show only articles within StartDate and EndDate
        [Authorize]
        public async Task<IActionResult> Articles()
        {
            var today = DateTime.UtcNow.Date;
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(); // If user is not logged in, deny access
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "admin");

            var articles = await _context.Articles
                .Where(a => a.StartDate.Date <= today && a.EndDate.Date >= today)
                .OrderByDescending(a => a.CreateDate)
                .ToListAsync();

            if (!isAdmin)
            {
                // If the user is NOT an admin, only show their own articles
                articles = articles.Where(a => a.ContributorUsername == user.Email).ToList();
            }

            return View(articles);
        }

        // View a single article
        public async Task<IActionResult> ArticleDetails(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();
            return View(article);
        }

        // Only logged-in contributors can create articles
        [Authorize(Roles = "contributor")]
        public IActionResult CreateArticle()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "contributor")]
        public async Task<IActionResult> CreateArticle(Article article)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Console.WriteLine("❌ ERROR: User is null - Are you logged in?");
                ModelState.AddModelError("", "You must be logged in to create an article.");
                return View(article);
            }

            // Manually assign ContributorUsername before validation
            article.ContributorUsername = user.Email;
            article.CreateDate = DateTime.UtcNow;

            Console.WriteLine($"✅ Assigned ContributorUsername: {article.ContributorUsername}");

            // 🔥 Remove validation error for ContributorUsername since we are manually setting it
            ModelState.Remove("ContributorUsername");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState is NOT valid! Showing validation errors...");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return View(article);
            }

            // Save to database
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Article Created: {article.Title}, Contributor: {article.ContributorUsername}");
            return RedirectToAction(nameof(Articles));
        }

        // Contributors can delete only their own articles
        [Authorize(Roles = "contributor")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            bool isOwner = user.Email == article.ContributorUsername;

            if (!isOwner)
            {
                return Forbid(); // Only allow the owner to delete
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Articles));
        }

        // ✅ MOVE EditArticle METHODS INSIDE THE CONTROLLER CLASS ✅

        // GET: Edit article page
        [Authorize(Roles = "contributor")]
        public async Task<IActionResult> EditArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            bool isOwner = user.Email == article.ContributorUsername;
            bool isAdmin = await _userManager.IsInRoleAsync(user, "admin");

            if (!isOwner && !isAdmin)
            {
                return Forbid(); // Only allow the owner or an admin to edit
            }

            return View(article);
        }

        // POST: Save the edited article
        [HttpPost]
        [Authorize(Roles = "contributor")]
        public async Task<IActionResult> EditArticle(Article article)
        {
            var user = await _userManager.GetUserAsync(User);
            var existingArticle = await _context.Articles.FindAsync(article.ArticleId);

            if (existingArticle == null)
            {
                return NotFound();
            }

            bool isOwner = user.Email == existingArticle.ContributorUsername;
            bool isAdmin = await _userManager.IsInRoleAsync(user, "admin");

            if (!isOwner && !isAdmin)
            {
                return Forbid(); // Prevent unauthorized edits
            }

            // Update only editable fields
            existingArticle.Title = article.Title;
            existingArticle.Body = article.Body;
            existingArticle.StartDate = article.StartDate;
            existingArticle.EndDate = article.EndDate;

            _context.Update(existingArticle);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Articles));
        }
    } // ✅ MAKE SURE THIS IS THE FINAL CLOSING BRACE OF THE CLASS ✅
}
