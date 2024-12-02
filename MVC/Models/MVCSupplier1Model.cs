using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class MVCSupplier1Model
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "ProductName is required")]
        public string Productname { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int Price { get; set; }

        public decimal TotalAmount { get; set; }
        public int PurchaseId { get; set; }
    }
}