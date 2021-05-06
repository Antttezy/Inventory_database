using Inventory_database.Models;
using System.Collections.Generic;

namespace Inventory_database.ViewModels
{
    public class AdministrationViewModel
    {
        public IEnumerable<User> Users { get; set; }

        public PagingViewModel Page { get; set; }
    }
}
