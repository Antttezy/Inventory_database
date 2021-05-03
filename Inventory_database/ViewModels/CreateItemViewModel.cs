using Inventory_database.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory_database.ViewModels
{
    public class CreateItemViewModel
    {
        public SelectList Types { get; set; }

        public SelectList Rooms { get; set; }

        public StorageItem InnerModel { get; set; }
    }
}
