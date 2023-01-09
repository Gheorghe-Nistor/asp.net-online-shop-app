using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using OnlineShopApp.Data;
using OnlineShopApp.Models;
using System.Diagnostics;
using System.Xml.Linq;
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
            ViewBag.isUser = User.IsInRole("User") || false;
            ViewBag.isCollaborator = User.IsInRole("Collaborator") || false;
            ViewBag.isAdmin = User.IsInRole("Admin") || false;
        }

        // Se afișează toate produsele validate împreună cu categoria din care fac parte
        // HttpGet implicit
        public IActionResult Index()
        {
            SetAccesRights();
            var products = db.Products.Include("Category")
                                      .Include("User")
                                      .Where(p => p.Status == true);

            foreach (var product in products)
            {
                var rating = from c in db.Comments
                             where c.ProductId == product.Id
                             select c.Rating;
                product.Rating = rating.Average();
            }
            db.SaveChanges();



            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                products = products.Where(p => p.Title.Contains(search));
            }

            var order = "";

            
            if (Convert.ToString(HttpContext.Request.Query["order"]) != null)
            {
                order = Convert.ToString(HttpContext.Request.Query["order"]);
                switch (order)
                {
                    case "Pret ascendent":
                        products = products.OrderBy(p => (p.Price - p.Price * p.Discount));
                        break;
                    case "Pret descendent":
                        products = products.OrderByDescending(p => (p.Price - p.Price * p.Discount));
                        break;
                    case "Rating ascendent":
                        //products = products.OrderBy(p => p.Rating);
                        products = products.OrderBy(p => p.Rating ?? double.MaxValue);
                        break;
                    case "Rating descendent":
                        products = products.OrderByDescending(p => p.Rating);
                        break;
                }
            }



            ViewBag.Products = products;

            ViewBag.SearchString = search;


            if (User.IsInRole("Admin"))
            {
                var unvalidatedProducts = db.Products.Include("Category")
                                          .Include("User")
                                          .Where(p => p.Status == false);
                ViewBag.UnvalidatedProducts = unvalidatedProducts.Count();
            }
            return View();
        }

        // Se afișează toate produsele adăugate de către contribuitori care nu au fost încă validate
        // HtppGet Implicit
        [Authorize(Roles ="Admin")]
        public IActionResult Validate(int? id)
        {
            SetAccesRights();
            if (id == null)
            {
                var products = db.Products.Include("Category")
                                          .Include("User")
                                          .Where(p => p.Status == false)
                                          .OrderBy(p => p.Price);
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
                    Product product = db.Products.Where(p => p.Id == id)
                                                 .First();
                    product.Status = true;
                    db.SaveChanges();
                    TempData["message"] = $"Produsul a fost validat cu succes!";
                    TempData["messageType"] = "alert-success";
                }
                catch(Exception)
                {
                    TempData["message"] = $"Nu s-a putut efectua operația de validare produs. Produsul dat nu a fost găsit în baza de date!";
                    TempData["messageType"] = "alert-danger";
                }
                return RedirectToAction("Index");
            }  
        }
        // Se afișează un singur produs cu id-ul dat, respectiv afișează mesaj de eroare în index
        // HtppGet Implicit
        public IActionResult Show(int id)
        {
            SetAccesRights();
            try
            {
                Product product = db.Products.Include("Category")
                                             .Include("User") 
                                             .Include("Comments")
                                             .Include("Comments.User")
                                             .Where(p => p.Id == id)
                                             .First();
                var rating = from c in db.Comments
                                where c.ProductId == product.Id
                                select c.Rating;
                product.Rating = rating.Average();
                db.SaveChanges();
                ViewBag.Product = product;
                return View(product);
            }
            catch (Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de afișare produs. Produsul dat nu a fost găsit în baza de date!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("index");
            }
        }


        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show([FromForm] Comment comm)
        {
            comm.Date = DateTime.Now;
            comm.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                try
                {
                    db.Comments.Add(comm);
                    db.SaveChanges();
                    TempData["message"] = $"Recenzia a fost adăugată cu succes!";
                    TempData["messageType"] = "alert-success";
                    return Redirect("/Products/Show/" + comm.ProductId);
                }
                catch (Exception)
                {
                    return Redirect("/Products/Show/" + comm.ProductId);
                }
            }
            else
            {
                Product product = db.Products.Include("Category")
                                         .Include("User")
                                         .Include("Comments")
                                         .Include("Comments.User")
                                         .Where(p => p.Id == comm.ProductId)
                                         .First();


                SetAccesRights();

                return View(comm);
            }

        }

        // Se afișează un view cu un formular pentru adăugare produs doar dacă există cel puțin o categorie
        // HttpGet Implicit
        [Authorize(Roles = "Collaborator,Admin")]
        public IActionResult New()
        {
            SetAccesRights();
            if (db.Categories.Count() != 0)
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
        public async Task<IActionResult> New(Product product, IFormFile Image)
        {
            product.CreatedAt = DateTime.Now;
            product.UserId = _userManager.GetUserId(User);
            product.CategoriesList = GetAllCategories();
            if(Image != null)
            {
                using (var stream = new MemoryStream())
                {
                    await Image.CopyToAsync(stream);
                    product.Image = stream.ToArray();
                }
            }
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                TempData["message"] = $"Produsul a fost adaugat cu succes!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", new {@id = product.Id});
            }
            return View(product);
        }
        // Editarea unui produs cu id-ul dat
        [Authorize(Roles = "Collaborator,Admin")]
        public IActionResult Edit(int id)
        {
            SetAccesRights();
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
                TempData["message"] = $"Nu s-a putut efectua operația de editare produs. Produsul dat nu a fost găsit în baza de date!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }
        // Se editează produsul din baza de date cu noile date din requestProdus
        [HttpPost]
        [Authorize(Roles = "Collaborator,Admin")]
        public async Task<IActionResult> Edit(int id, Product requestProduct, IFormFile? Image)
        {
            Product product = db.Products.Include("Category")
                                         .Include("User")
                                         .Where(p => p.Id == id)
                                         .First();
            if (ModelState.IsValid)
            {
                if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    if (Image != null)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await Image.CopyToAsync(stream);
                            product.Image = stream.ToArray();
                        }
                    }
                    product.Title = requestProduct.Title;
                    product.Description = requestProduct.Description;
                    product.Price = requestProduct.Price;
                    product.Discount = requestProduct.Discount;
                    product.CategoryId = requestProduct.CategoryId;
                    db.SaveChanges();
                    TempData["message"] = $"Produsul a fost editat cu succes!";
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
                Product product = db.Products.Include("Comments")
                                             .Where(p => p.Id == id)
                                             .First();
                if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    var cartItems = db.ShoppingCarts.Where(i => i.ProductId == id);
                    foreach (var cartItem in cartItems)
                    {
                        db.ShoppingCarts.Remove(cartItem);
                    }
                    db.Products.Remove(product);
                    db.SaveChanges();
                    TempData["message"] = $"Produsul a fost șters cu succes!";
                    TempData["messageType"] = "alert-success";
                }
                else
                {
                    TempData["message"] = $"Nu aveți dreptul să ștergeți un produs care nu vă aparține!";
                    TempData["messageType"] = "alert-danger";
                }
            }
            catch (Exception)
            {
                TempData["message"] = $"Nu s-a putut efectua operația de ștergere produs. Produsul dat nu a fost găsit în baza de date!";
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
