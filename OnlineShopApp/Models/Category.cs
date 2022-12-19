using System.ComponentModel.DataAnnotations;

namespace OnlineShopApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string? CategoryName { get; set; }
    }
}
