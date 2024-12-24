// Models/ProductViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalogApp.Models
{
    public class ProductViewModel
    {
        [Display(Name = "Filter by Category")]
        public string? SelectedCategory { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
