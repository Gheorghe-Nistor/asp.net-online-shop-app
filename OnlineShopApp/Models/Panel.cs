using System.ComponentModel.DataAnnotations;

namespace OnlineShopApp.Models
{
    public class Panel
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? CurrentRole { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
