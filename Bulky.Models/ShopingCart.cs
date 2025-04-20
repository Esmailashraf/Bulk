using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Models
{
    public class ShopingCart
    {
        [Key]
        public int Id { get; set; }

        [Range(1,1000,ErrorMessage ="out of range")]
        public int Count { get; set; }

        public int ProdunctId { get; set; }

        [ValidateNever]
        [ForeignKey("ProdunctId")]
        public Product Product { get; set; }

        public string ApplicationUserId { get; set; }
        [ValidateNever]
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        [NotMapped] // not add to database
        public double Price { get; set; }

    }
}
