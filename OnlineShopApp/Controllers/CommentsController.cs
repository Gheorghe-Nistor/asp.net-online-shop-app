using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShopApp.Data;
using OnlineShopApp.Models;
using System.Data;

namespace OnlineShopApp.Controllers
{
    public class CommentsController : Controller
    {

        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(
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

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id)
        {
            SetAccesRights();
            try
            {
                Comment comment = db.Comments.Find(id);
                if (comment.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    return View(comment);
                }
                else
                {
                    TempData["message"] = "Nu aveți dreptul să editați recenzia";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Products");
                }
                    
            }
            catch (Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de editare recenzie.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comment = db.Comments.Find(id);

            if (comment.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    comment.Content = requestComment.Content;
                    comment.Rating = requestComment.Rating;
                    db.SaveChanges();
                    TempData["message"] = $"Recenzia a fost modificată cu succes!";
                    TempData["messageType"] = "alert-success";
                    return Redirect("/Products/Show/" + comment.ProductId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            else
            {
                TempData["message"] = "Nu aveți dreptul să faceți modificări";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                if (comment.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    db.Comments.Remove(comment);
                    db.SaveChanges();
                    TempData["message"] = $"Recenzia a fost ștearsă cu succes!";
                    TempData["messageType"] = "alert-success";
                    return Redirect("/Products/Show/" + comment.ProductId);
                }
                else
                {
                    TempData["message"] = "Nu aveți dreptul să ștergeți recenzia";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Products");
                }
            }

            catch(Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de ștergere recenzie.";
                TempData["messageType"] = "alert-danger";
            }
            return Redirect("/Products");
        }
    }
}
