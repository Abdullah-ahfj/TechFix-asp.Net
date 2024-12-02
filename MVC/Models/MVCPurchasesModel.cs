using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class MVCPurchasesModel
    {
        public int PurchaseId { get; set; }
        public string Productname { get; set; }
        public string Suppliername { get; set; }
        public int PurchasedQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}