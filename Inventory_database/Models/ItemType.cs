using System.ComponentModel.DataAnnotations;

namespace Inventory_database.Models
{
    public class ItemType
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
