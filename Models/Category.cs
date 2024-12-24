// Models/Category.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalogApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Product> Products { get; set; }

        public Category()
        {
            Name = string.Empty;
            Products = new List<Product>();
        }
    }
}
