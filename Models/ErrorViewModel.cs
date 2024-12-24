// Models/ErrorViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace ProductCatalogApp.Models
{
    public class ErrorViewModel
    {
        [Required]
        public string RequestId { get; set; } = string.Empty; // Initialized to prevent null

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
