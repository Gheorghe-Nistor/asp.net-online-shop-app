using Microsoft.AspNetCore.Mvc;
using OnlineShopApp.Data;
using OnlineShopApp.Models;

namespace OnlineShopApp.Controllers
{
    public class CommentsController : Controller
    {

        private readonly ApplicationDbContext db;

        public CommentsController(ApplicationDbContext context)
        {
            db = context;
        }

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
        public IActionResult Edit(int id)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                return View(comment);
            }
            catch (Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de editare comentariul. ID-ul {id} nu corespounde niciunui comentariu din baza de date!";
                TempData["messageType"] = "alert-danger";
            }
            return Redirect("/Products");
        }

        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comment = db.Comments.Find(id);
            if(ModelState.IsValid)
            {
                comment.Content = requestComment.Content;
                db.SaveChanges();
                TempData["message"] = $"Comentariul cu id-ul {id} a fost modificat cu succes!";
                TempData["messageType"] = "alert-success";
                return Redirect("/Products/Show/" + comment.ProductId);
            }
            return View(requestComment);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                db.Comments.Remove(comment);
                db.SaveChanges();
                TempData["message"] = $"Comentariul cu id-ul {id} a fost șters cu succes!";
                TempData["messageType"] = "alert-success";
                //return Redirect("/Products/Show/" + comment.Id);
                return Redirect("/Products/Show/" + comment.ProductId);
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
