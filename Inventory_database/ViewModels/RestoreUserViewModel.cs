using System.ComponentModel.DataAnnotations;

namespace Inventory_database.ViewModels
{
    public class RestoreUserViewModel
    {
        [Required]
        public int Id { get; set; }

        [StringLength(32, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
