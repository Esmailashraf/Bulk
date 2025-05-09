﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ValidateNever]
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ShoppingDate { get; set; }

        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }

        public DateTime PaymentTime { get; set; }
        public DateOnly PaymentDueTime { get; set; }

        public string? PaymentIntentId { get; set; }

        public string? SessionId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]

        public string StreetAddress { get; set; }
        [Required]

        public string City { get; set; }
        [Required]

        public string PhoneNumber { get; set; }
        [Required]


        public string State { get; set; }
        [Required]

        public string PostalCode { get; set; }



    }
}
