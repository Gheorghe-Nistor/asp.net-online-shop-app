using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShopApp.Data;
using OnlineShopApp.Models;
using System.Data;

namespace OnlineShopApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PanelsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public PanelsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        private void SetAccesRights()
        {
            ViewBag.CurrentUser = _userManager.GetUserId(User);
            ViewBag.isUser = User.IsInRole("User") || false;
            ViewBag.isCollaborator = User.IsInRole("Collaborator") || false;
            ViewBag.isAdmin = User.IsInRole("Admin") || false;
        }
        public IActionResult Index()
        {

            return View();
        }
    }
}
