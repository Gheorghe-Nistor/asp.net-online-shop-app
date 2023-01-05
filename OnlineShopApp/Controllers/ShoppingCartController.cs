using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopApp.Data;
using OnlineShopApp.Models;

namespace OnlineShopApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
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

        [Authorize(Roles="User,Collaborator,Admin")]
        public IActionResult Index()
        {
            SetAccesRights();
            string currentUser = _userManager.GetUserId(User);
            if (currentUser == null)
            {
                TempData["message"] = $"Trebuie să fi autentificat pentru a avea acces la coșul de cumpărături!";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Identity/Account/Login");
            }
            var cartItems = db.ShoppingCarts.Where(c => c.UserId == currentUser)
                                            .Include("Product")        
                                            .Include("Product.User");
            ViewBag.CartItems = cartItems;
            return View();
        }

        public IActionResult Add(int id)
        {
            string currentUser = _userManager.GetUserId(User);
            if(currentUser == null)
            {
                TempData["message"] = $"Trebuie să fi autentificat pentru a adăuga un produs în coșul tău de cumpărături!";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Identity/Account/Login");
            }
            try
            { 
                // Produsul cu id-ul dat există în baza de date
                Product product = db.Products.Where(p => p.Id == id)
                                             .First();
                try
                {
                    // Produsul cu id-ul dat există deja in coșul de cumpărături al user-ul așa că vom incrementa cantitatea cu 1
                    ShoppingCart cartItem = db.ShoppingCarts.Where(c => c.UserId == currentUser && c.ProductId == product.Id)
                                                            .First();
                    cartItem.Quantity += 1;
                    db.SaveChanges();
                }
                catch
                {
                    // Produsul cu id-ul dat NU există în coșul de cumpărături al user-ului
                    ShoppingCart cartItem = new ShoppingCart();
                    cartItem.UserId = currentUser;
                    cartItem.ProductId = product.Id;
                    cartItem.Quantity = 1;
                    db.ShoppingCarts.Add(cartItem);
                    db.SaveChanges();
                }
                TempData["message"] = $"Produsul a fost adaugat în coșul dvs. de cumpărături cu succes!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "Products", new { @id = product.Id });
            }
            catch{
                // Produsul cu id-ul dat NU există în baza de date
                TempData["message"] = $"Nu s-a putut efectua operația de adăugare produs în coș. Produsul dat nu a fost găsit în baza de date!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }
        }
        public IActionResult Edit(int id, int value)
        {
            string currentUser = _userManager.GetUserId(User);
            if (currentUser == null)
            {
                TempData["message"] = $"Trebuie să fi autentificat pentru a edita un produs din coșul tău de cumpărături!";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Identity/Account/Login");
            }
            try
            {
                // Produsul cu id-ul dat există în coșul de cumpărături al user-ului
                ShoppingCart cartItem = db.ShoppingCarts.Where(c => c.UserId == currentUser && c.ProductId == id)
                                                        .First();
                cartItem.Quantity += value;
                if (cartItem.Quantity == 0)
                {
                    db.ShoppingCarts.Remove(cartItem);
                    TempData["message"] = $"Produsul a fost șters cu succes din coșul dvs. de cumpărături!";
                }
                else
                    TempData["message"] = $"Cantitatea produsului din coșul dvs. de cumpărături a fost modificată cu succes!";
                TempData["messageType"] = "alert-success";
                db.SaveChanges();
            }
            catch
            {
                // Produsul cu id-ul dat NU există în coșul de cumpărături al user-ului
                TempData["message"] = $"Nu s-a putut efectua operația de editare a cantității produsului din coș. Produsul dat nu se află în coșul dvs. de cumpărături!";
                TempData["messageType"] = "alert-danger";   
            }
            return RedirectToAction("Index", "ShoppingCart");
        }
    }
}
