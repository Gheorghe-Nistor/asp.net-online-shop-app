using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public DateTime Date { get; set; }

        // image, colecție review-uri
        // categorie
    }
}
