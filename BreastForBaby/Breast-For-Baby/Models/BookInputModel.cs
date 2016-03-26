using System.ComponentModel.DataAnnotations;

namespace Breast_For_Baby.Models
{
    public class BookInputModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string ContactMethod { get; set; }

        public string Situation { get; set; }
    }
}