﻿using Inventory_database.Models;
using System.Collections.Generic;

namespace Inventory_database.ViewModels
{
    public class ItemsViewModel
    {
        public IEnumerable<StorageItem> Items { get; set; }

        public int Page { get; set; }
    }
}
