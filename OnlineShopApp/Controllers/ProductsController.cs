using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopApp.Data;
using OnlineShopApp.Models;
using System.Reflection.Metadata.Ecma335;

namespace OnlineShopApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        public ProductsController(ApplicationDbContext context)
        {
            db = context;
        }
        // Se afișează toate produsele validate împreună cu categoria din care fac parte
        // HttpGet implicit
        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message     = TempData["message"].ToString();
                if(TempData.ContainsKey("messageType"))
                    ViewBag.MessageType = TempData["messageType"].ToString();
            }
            var products = db.Products.Include("Category")
                                      .Where(p => p.Status == true);
            ViewBag.Products = products;
            return View();
        }

        // Se afișează toate produsele adăugate de către contribuitori care nu au fost încă validate
        // HtppGet Implicit
        public IActionResult Validate()
        {
            var products = db.Products.Include("Category")
                                      .Where(p => p.Status == false);
            ViewBag.Products = products;
            return View();
        }

        /*public IActionResult Validate(int id)
        {
            var product = db.Products.Where(p => p.Id == id)
                                     .First();
            product.Status = true;
            TempData["message"] = $"Produsul cu id-ul {id} a fost validat!";
            TempData["messageType"] = "alert-succes";
            db.SaveChanges();
            return View("Index");
        } */
        public IActionResult Show(int? id)
        {
            var product = db.Products.Find(id);
            ViewBag.Product = product;
            return View();
        }
        
        public IActionResult New()
        {
            Product product = new Product();
            product.CategoriesList = GetAllCategories();
            return View(product);
        }
        [HttpPost]
        public IActionResult New(Product product)
        {
            product.CreatedAt = DateTime.Now;
           // if (ModelState.IsValid)
           // {
                db.Products.Add(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost adaugat cu succes. În maxim 24h un administrator va valida sau va șterge anunțul dvs.";
                TempData["messageType"] = "alert-success";
               
            //}
            return RedirectToAction("Index");
        }
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            return selectList;
        }
        public IActionResult Edit(int id)
        {
            Product product = db.Products.Include("Category")
                                          .Where(p => p.Id == id)
                                          .First();
            product.CategoriesList = GetAllCategories();
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(int id, Product requestProduct)
        {
            Product product = db.Products.Find(id);
            if (ModelState.IsValid)
            {
                product.Title = requestProduct.Title;
                product.CategoryId = requestProduct.CategoryId;
                TempData["Message"] = "Produsul a fost modificat";
                return RedirectToAction("Index");
            }
            requestProduct.CategoriesList = GetAllCategories();
            return View(requestProduct);

        }
        public IActionResult Delete(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            TempData["message"] = "Produsul a fost șters cu succes.";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }
    }
}
