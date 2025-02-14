using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
namespace MyApp.Namespace
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<ApplicationUser> _userManager;


        public AdminController(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Admin()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public ActionResult EditUser(string id)
        {
            Console.WriteLine("-----------------");
            Console.WriteLine(_roleManager.Roles.ToList());
            Console.WriteLine("-----------------");
            ViewData["Roles"] = new SelectList(_roleManager.Roles.ToList(), "Roles", "Roles");
            return View();
        }

    }
}
