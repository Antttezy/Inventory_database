using System.ComponentModel.DataAnnotations;

namespace Inventory_database.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [StringLength(32, MinimumLength = 4)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 4)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 4)]
        public string RepPassword { get; set; }
    }
}
