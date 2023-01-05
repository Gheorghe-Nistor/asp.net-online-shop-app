using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopApp.Data;
using OnlineShopApp.Models;
using System.Xml.Schema;

namespace OnlineShopApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private void SetAccesRights()
        {
            ViewBag.CurrentUser = _userManager.GetUserId(User);
            ViewBag.isUser = User.IsInRole("User");
            ViewBag.isCollaborator = User.IsInRole("Collaborator");
            ViewBag.isAdmin = User.IsInRole("Admin");
        }

        // Se afișează toate produsele validate împreună cu categoria din care fac parte
        // HttpGet implicit
        public IActionResult Index()
        {
            var products = db.Products.Include("Category")
                                      .Include("User")
                                      .Where(p => p.Status == true);
            ViewBag.Products = products;
            SetAccesRights();
            return View();
        }

        // Se afișează toate produsele adăugate de către contribuitori care nu au fost încă validate
        // HtppGet Implicit
        [Authorize(Roles ="Admin")]
        public IActionResult Validate(int? id)
        {
            if (id == null)
            {
                var products = db.Products.Include("Category")
                                          .Include("User")
                                          .Where(p => p.Status == false);
                if (products.Count() > 0) {
                    ViewBag.Products = products;
                    return View();
                }
                TempData["message"] = "Toate produsele din baza de date sunt validate!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");

            } else
            {
                try 
                {
                    var product = db.Products.Where(p => p.Id == id)
                                              .First();
                    product.Status = true;
                    db.SaveChanges();
                    TempData["message"] = $"Produsul cu id-ul {id} a fost validat cu succes!";
                    TempData["messageType"] = "alert-success";
                }
                catch(Exception)
                {
                    TempData["message"] = $"Nu s-a putut efectua operația de validare produs. ID-ul {id} nu corespounde niciunui produs din baza de date!";
                    TempData["messageType"] = "alert-danger";
                }
                return RedirectToAction("Index");
            }  
        }
        // Se afișează un singur produs cu id-ul dat, respectiv afișează mesaj de eroare în index
        // HtppGet Implicit
        public IActionResult Show(int id)
        {
            try
            {
                Product product = db.Products.Include("Category")
                                             .Include("User") 
                                             .Include("Comments")
                                             .Where(p => p.Id == id)
                                             .First();
                ViewBag.Product = product;
                SetAccesRights();
                return View();
            }
            catch (Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de afișare produs. ID-ul {id} nu corespounde niciunui produs din baza de date!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("index");
            }
        }

        // Se afișează un view cu un formular pentru adăugare produs doar dacă există cel puțin o categorie
        // HttpGet Implicit
        [Authorize(Roles = "Collaborator,Admin")]
        public IActionResult New()
        {
            if(db.Categories.Count() != 0)
            {
                Product product = new Product();
                product.CategoriesList = GetAllCategories();
                return View(product);

            }               
            TempData["message"] = $"Nu s-a putut efectua operația de adăugare produs. Nu există nici o categorie în baza de date!";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("index");
        }


        // Se adaugă produsul în baza de date dacă datele acestuia respectă toate constrângerile
        [HttpPost]
        [Authorize(Roles = "Collaborator,Admin")]
        public IActionResult New(Product product)
        {
            product.CreatedAt = DateTime.Now;
            product.UserId = _userManager.GetUserId(User);
            product.CategoriesList = GetAllCategories();
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                TempData["message"] = $"Produsul cu id-ul {product.Id} a fost adaugat cu succes!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", new {@id = product.Id});
            }
            return View(product);
        }
        // Editarea unui produs cu id-ul dat
        [Authorize(Roles = "Collaborator,Admin")]
        public IActionResult Edit(int id)
        {
            try
            {
                Product product = db.Products.Include("Category")
                                             .Include("User")
                                             .Where(p => p.Id == id)
                                             .First();
                product.CategoriesList = GetAllCategories();
                if(product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                    return View(product);
                TempData["message"] = $"Nu aveți dreptul să aduceți modificări asupra unui produs care nu vă aparține!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de editare produs. ID-ul {id} nu corespounde niciunui produs din baza de date!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }
        // Se editează produsul din baza de date cu noile date din requestProdus
        [HttpPost]
        [Authorize(Roles = "Collaborator,Admin")]
        public IActionResult Edit(int id, Product requestProduct)
        {
            Product product = db.Products.Include("Category")
                                         .Include("User")
                                         .Where(p => p.Id == id)
                                         .First();
            if (ModelState.IsValid)
            {
                if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    product.Title = requestProduct.Title;
                    product.Description = requestProduct.Description;
                    product.Price = requestProduct.Price;
                    product.Discount = requestProduct.Discount;
                    product.CategoryId = requestProduct.CategoryId;
                    db.SaveChanges();
                    TempData["message"] = $"Produsul cu id-ul {id} a fost editat cu succes!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }
                TempData["message"] = $"Nu aveți dreptul să aduceți modificări asupra unui produs care nu vă aparține!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            requestProduct.CategoriesList = GetAllCategories();
            return View(requestProduct);

        }
        // Se șterge produsul cu id-ul dat dacă acesta există în baza de date
        // HtppGet Implicit
        [Authorize(Roles = "Collaborator,Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                Product product = db.Products.Include("Category")
                                          .Where(p => p.Id == id)
                                          .First();
                if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    db.Products.Remove(product);
                    db.SaveChanges();
                    TempData["message"] = $"Produsul cu id-ul {id} a fost șters cu succes!";
                    TempData["messageType"] = "alert-success";
                }
                TempData["message"] = $"Nu aveți dreptul să ștergeți un produs care nu vă aparține!";
                TempData["messageType"] = "alert-danger";
            }
            catch (Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de ștergere produs. ID-ul {id} nu corespounde niciunui produs din baza de date!";
                TempData["messageType"] = "alert-danger";
            }
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
    }
}
