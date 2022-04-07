using LessonMigration.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LessonMigration.ViewModels.Admin
{
    public class ProductUpdateVM
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Count { get; set; }
        public int CategoryId { get; set; }
        public ICollection<ProductImage> Images { get; set; }
        
        public List<IFormFile> Photos { get; set; }
    }
}
