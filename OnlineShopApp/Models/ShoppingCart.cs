using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopApp.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? ProductId { get; set; }
        [DefaultValue(1)]
        public int Quantity { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Product? Product { get; set; }
    }
}
