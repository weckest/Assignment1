using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol;
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
            ViewData["id"] = id;
            ViewData["Roles"] = new SelectList(_roleManager.Roles.ToList(), "Id", "Name");
            Console.WriteLine(ViewData["Roles"].ToJson() + "\nSomething-----------------");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string id, string role)
        {
            Console.WriteLine("-----------------");
            Console.WriteLine(id);
            Console.WriteLine(role);
            Console.WriteLine("-----------------");
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            // try {
                await _userManager.RemoveFromRolesAsync(user, roles.ToArray());
                var roleName = _roleManager.Roles.FirstOrDefault(r => r.Id == role)?.Name;
                await _userManager.AddToRoleAsync(user, roleName);
            // } catch (Exception e) {
                // Console.WriteLine(e.Message);
            // }
            
            return RedirectToAction("Admin");
        }

    }
}
