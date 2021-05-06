using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Inventory_database.ViewModels
{
    public class EditUserViewModel
    {
        public int User { get; set; }

        public List<int> RolesId { get; set; } = new List<int>();

        public MultiSelectList UserRoles { get; set; }
    }
}
