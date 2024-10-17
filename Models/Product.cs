using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CrudProject.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public String? PName { get; set; }

        public String? Description { get; set; }

        [Precision(16, 2)]
        public decimal Price {  get; set; }
        public String? ImageFileName { get; set; }

        public string? UserId { get; set; }

        public string? MobileNumber { get; set; }

        public string? Email {  get; set; }
    }
}
