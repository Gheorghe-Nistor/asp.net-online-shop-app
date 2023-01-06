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

        /*
        [HttpPost]
        public IActionResult New(Comment comm)
        {
            comm.Date = DateTime.Now;

            try
            {
                db.Comments.Add(comm);
                db.SaveChanges();
                TempData["message"] = $"Comentariul cu id-ul {comm.Id} a fost adăugat cu succes!";
                TempData["messageType"] = "alert-success";
                return Redirect("/Products/Show/" + comm.ProductId);
            }

            catch (Exception)
            {
                return Redirect("/Products/Show/" + comm.ProductId);
            }

        }
        */

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                if (comment.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    return View(comment);
                }
                else
                {
                    TempData["message"] = "Nu aveți dreptul să editați comentariul";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Products");
                }
                    
            }
            catch (Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de editare comentariul. ID-ul {id} nu corespounde niciunui comentariu din baza de date!";
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
                    db.SaveChanges();
                    TempData["message"] = $"Comentariul cu id-ul {id} a fost modificat cu succes!";
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
                    TempData["message"] = $"Comentariul cu id-ul {id} a fost șters cu succes!";
                    TempData["messageType"] = "alert-success";
                    return Redirect("/Products/Show/" + comment.ProductId);
                }
                else
                {
                    TempData["message"] = "Nu aveți dreptul să ștergeți comentariul";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Products");
                }
            }

            catch(Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de ștergere comentariul. ID-ul {id} nu corespounde niciunui comentariu din baza de date!";
                TempData["messageType"] = "alert-danger";
            }
            return Redirect("/Products");
        }
    }
}
