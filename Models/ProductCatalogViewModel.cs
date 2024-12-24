// Models/ProductCatalogViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ProductCatalogApp.Models
{
    public class ProductCatalogViewModel
    {
        // For filtering
        public int? SelectedCategoryId { get; set; }

        // List of products to display
        public List<Product> Products { get; set; } = new List<Product>();

        // List of categories for the dropdown
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
