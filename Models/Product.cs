// Models/Product.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalogApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // Initialized to prevent null

        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; } // Duration in days
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        [Required]
        public string CreatedByUserId { get; set; } = string.Empty; // Initialized to prevent null

        public ApplicationUser? CreatedByUser { get; set; }
    }
}
