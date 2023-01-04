using OnlineShopApp.Data;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
