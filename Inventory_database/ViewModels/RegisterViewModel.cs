using System.ComponentModel.DataAnnotations;

namespace Inventory_database.ViewModels
{
    public class RegisterViewModel
    {
        public string Code { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 4)]
        public string Password { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 4)]
        public string ConfirmPassword { get; set; }
    }
}
