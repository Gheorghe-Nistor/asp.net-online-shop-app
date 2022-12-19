using System.ComponentModel.DataAnnotations;

namespace OnlineShopApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
<<<<<<< HEAD
        public string? CategoryName { get; set; }
=======

        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
>>>>>>> a372e4ea49dd9b2da2ce6f888fd0620544c752ef
    }
}
