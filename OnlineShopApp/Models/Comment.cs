using OnlineShopApp.Data;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public string? Content { get; set; }
        public DateTime Date { get; set; }
        public int? ProductId { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; } // un comentariu aparține unui singur utilizator
        public virtual Product? Product { get; set; }

        public int? Rating { get; set; }
    }
}
