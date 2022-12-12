using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.ComponentModel.DataAnnotations;
using static Humanizer.On;

namespace OnlineShopApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
        [MinLength(5, ErrorMessage = "Titlul nu poate avea mai puțin de 5 caractere")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Descrierea este obligatorie")]
        [MinLength(30, ErrorMessage = "Descrierea nu poate avea mai puțin de 50 caractere")]
        public string? Description { get; set; }
        public double? Price { get; set; }
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        // image, colecție review-uri
        // categorie
    }
}
