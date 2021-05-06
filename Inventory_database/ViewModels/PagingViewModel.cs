using System;

namespace Inventory_database.ViewModels
{
    public class PagingViewModel
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int ItemsPerPage { get; set; }

        public PagingViewModel(int currentPage, int itemCount, int itemsPerPage)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;

            if (itemCount < itemsPerPage)
            {
                TotalPages = 1;
            }
            else
            {
                float pages = (float)itemCount / itemsPerPage;
                TotalPages = (int)MathF.Ceiling(pages);
            }
        }
    }
}
