using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models
{
    public class Category
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [DisplayName("Category Name")]
        [MaxLength(20)]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        [Range(1,100,ErrorMessage= "The field Display Order must be between 1 and 100.0")]
        public int DisplayOrder { get; set; }
    }
}
