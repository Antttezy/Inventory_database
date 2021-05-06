using System.ComponentModel.DataAnnotations;

namespace Inventory_database.ViewModels
{
    public class ChangeNameViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string SecondName { get; set; }

        [Required]
        public string ThirdName { get; set; }
    }
}
