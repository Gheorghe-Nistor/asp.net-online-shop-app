using Microsoft.AspNetCore.Mvc;
using OnlineShopApp.Data;
using OnlineShopApp.Models;

namespace OnlineShopApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoriesController(ApplicationDbContext contex)
        {
            db = contex;
        }
        public IActionResult Index()
        {
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;

            ViewBag.Categories = categories;
            return View();
        }

        public IActionResult New()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"].ToString();
            }
            return View();
        }

        [HttpPost]
        public IActionResult New(Category c)
        {
            var category = db.Categories.FirstOrDefault(cat => cat.CategoryName ==  c.CategoryName);
            if (category != null)
            {
                TempData["message"] = "Categoria deja exista";
                return RedirectToAction("New");
            }
            try
            {
                db.Categories.Add(c);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                return View();
            }
        }

        public IActionResult Edit(int id, Category requestCategory)
        {
            try
            {
                Category category = db.Categories.Find(id);
                {
                    category.CategoryName = requestCategory.CategoryName;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                ViewBag.Category = requestCategory;
                return View();
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
