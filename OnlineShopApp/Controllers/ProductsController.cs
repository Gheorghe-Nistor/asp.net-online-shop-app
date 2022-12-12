using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopApp.Data;

namespace OnlineShopApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        public ProductsController(ApplicationDbContext context)
        {
            db = context;
        }
        // Se afișează lista tuturor produselor împreună cu categoria din care fac parte
        // HttpGet implicit
        public IActionResult Index()
        {
            var products = db.Products.Include("Category");
            ViewBag.Products = products;
            return View();
        }
    }
}
