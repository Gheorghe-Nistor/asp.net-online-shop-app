﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopApp.Data;
using OnlineShopApp.Models;
using System.Data;

namespace OnlineShopApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PanelController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public PanelController(
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
            SetAccesRights();
            var users = from user in db.Users
                        join ur in db.UserRoles on user.Id equals ur.UserId
                        join r in db.Roles on ur.RoleId equals r.Id
                        select new { uname = user.UserName, role = r.Name, Id = user.Id };

  
            ViewBag.Users = users;

            return View();
        }

        [HttpPost]
        public IActionResult Index(string role, string id)
        {

        var rol_id =(from idr in db.Roles
                    where idr.Name == role
                    select idr).First().Id;


            var usr = db.UserRoles.Where(p=>p.UserId == id).First();

            db.UserRoles.Remove(usr);
            db.SaveChanges();
            usr.RoleId = rol_id;
            db.UserRoles.Add(usr);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
