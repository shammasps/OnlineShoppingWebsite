using Microsoft.EntityFrameworkCore;

namespace CrudProject.Models
{
    public class ProductViewModel
    {
        public String? PName { get; set; }

        public String? Description { get; set; }

        [Precision(16, 2)]
        public decimal Price { get; set; }
        public IFormFile? PImage { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }

    }
}
