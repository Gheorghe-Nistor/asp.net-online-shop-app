using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [DefaultValue(false)]
        public bool Status { get; set; }
        [Required(ErrorMessage = "Titlul este obligatoriu;")]
        [MaxLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere;")]
        [MinLength(3, ErrorMessage = "Titlul nu poate avea mai puțin de 3 caractere;")]

        public string? Title { get; set; }
        [Required(ErrorMessage = "Descrierea este obligatorie;")]
        [MinLength(30, ErrorMessage = "Descrierea nu poate avea mai puțin de 50 caractere;")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Prețul produsului este obligatoriu;")]
        [Range(1, 5000, ErrorMessage = "Prețul trebuie să fie cuprins între 1 și 5000 RON;")]
        public double Price { get; set; }
        [DefaultValue(0)]
        [Range(0, 1, ErrorMessage = "Valoarea discount-ului trebuie să fie cuprinsă între 0 și 1;")]
        public double? Discount { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[]? Image { get; set; }
        public int? CategoryId { get; set; } 
        public string? UserId { get; set; } 
        public virtual ApplicationUser? User { get; set; } // un produs aparține unui singur utilizator
        public virtual Category? Category { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? CategoriesList { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
