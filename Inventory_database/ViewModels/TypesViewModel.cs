using Inventory_database.Models;
using System.Collections.Generic;

namespace Inventory_database.ViewModels
{
    public class TypesViewModel
    {
        public IEnumerable<ItemType> Types { get; set; }

        public PagingViewModel Page { get; set; }
    }
}
