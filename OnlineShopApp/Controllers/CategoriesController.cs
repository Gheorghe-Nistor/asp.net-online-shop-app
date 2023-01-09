using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShopApp.Data;
using OnlineShopApp.Models;

namespace OnlineShopApp.Controllers
{
    [Authorize(Roles="Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoriesController(ApplicationDbContext contex)
        {
            db = contex;
        }

        // Sa se afiseze toate categoriile
        // HttpGet implicit
        public IActionResult Index()
        {
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;

            ViewBag.Categories = categories;
            return View();
        }

        // Sa se afiseze un view cu un formular pentru adaugarea unei noi categorii diferite de cele existente deja
        // HttpGet implicit
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        // Se adaugă produsul în baza de date dacă datele acestuia respectă toate constrângerile
        public IActionResult New(Category c)
        {
            var category = db.Categories.FirstOrDefault(cat => cat.CategoryName ==  c.CategoryName);
            if (category != null)
            {
                TempData["message"] = "Nu s-a putut efectua operația de adaugare categorie. Aceasta exista deja in baza de date!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                db.Categories.Add(c);
                db.SaveChanges();
                TempData["message"] = $"Categoria a fost adaugată cu succes!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            
            return View(c);
             
        }

        public IActionResult Edit(int id)
        {
            try
            {
                Category category = db.Categories.Find(id);
                return View(category);
            }
            catch(Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de editare categorie.";
                TempData["messageType"] = "alert-danger";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Edit(int id, Category requestCategory)
        {

            Category category = db.Categories.Find(id);
            if(ModelState.IsValid)
            {
                category.CategoryName = requestCategory.CategoryName;
                db.SaveChanges();
                TempData["message"] = $"Categoria a fost editată cu succes!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            return View(requestCategory);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            try
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
                db.SaveChanges();
                TempData["message"] = $"Categoria a fost ștearsă cu succes!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de ștergere categorie.";
                TempData["messageType"] = "alert-danger";
            }
            return RedirectToAction("Index");
        }
    }
}
