using Inventory_database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.ViewModels
{
    public class ItemsViewModel
    {
        public IEnumerable<StorageItem> Items { get; set; }

        public int Page { get; set; }
    }
}
