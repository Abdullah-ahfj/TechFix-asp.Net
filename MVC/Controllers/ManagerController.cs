using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager

        public ActionResult Home()
        {
            if (Session["Username"] == null)
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Login");
            }

            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Users").Result;
            var userList = response.Content.ReadAsAsync<IEnumerable<MVCLoginModel>>().Result;
            return View(userList);
        }

        public ActionResult Index()
        {
            if (Session["Username"] == null)
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Login");
            }

            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Users").Result;
            var userList = response.Content.ReadAsAsync<IEnumerable<MVCLoginModel>>().Result;
            return View(userList);
        }


        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new MVCLoginModel());
            }
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Users/" + id.ToString()).Result;
                return View(response.Content.ReadAsAsync<MVCLoginModel>().Result);
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(MVCLoginModel use)
        {
            if (use.UserId == 0)
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("Users", use).Result;
                TempData["SuccessMessage"] = "Saved Successfully";
            }
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PutAsJsonAsync("Users/" + use.UserId, use).Result;
                TempData["SuccessMessage"] = "Users Added Successfully";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            HttpResponseMessage response = GlobalVariables.WebApiClient.DeleteAsync("Users/" + id.ToString()).Result;
            TempData["SuccessMessage"] = "Deleted Successfully";
            return RedirectToAction("Index");
        }

        public ActionResult Purchase(string category)
        {
            IEnumerable<MVCSupplier1Model> ProductList;
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Products").Result;
            ProductList = response.Content.ReadAsAsync<IEnumerable<MVCSupplier1Model>>().Result;

            // Filter the products by category
            if (!string.IsNullOrEmpty(category))
            {
                ProductList = ProductList.Where(p => p.Category == category);
            }

            return View(ProductList);
        }

        [HttpPost]
        public ActionResult BuyProduct(int ProductId, int QuantityToBuy, int AvailableQuantity)
        {
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Products/" + ProductId.ToString()).Result;
            var product = response.Content.ReadAsAsync<MVCSupplier1Model>().Result;

            if (product != null)
            {
                if (QuantityToBuy <= AvailableQuantity)
                {
                    var purchase = new MVCPurchasesModel
                    {
                        Productname = product.Productname,
                        Suppliername = product.Username,
                        PurchasedQuantity = QuantityToBuy,
                        TotalAmount = product.Price * QuantityToBuy,
                        Status = "Requested"  // Status is now Requested, not directly Successful
                    };

                    HttpResponseMessage purchaseResponse = GlobalVariables.WebApiClient.PostAsJsonAsync("Purchases", purchase).Result;

                    if (purchaseResponse.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = $"Quotation for {QuantityToBuy} of {product.Productname} has been requested.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error sending purchase request.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Not enough stock available.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Product not found.";
            }

            return RedirectToAction("Purchase");
        }


        public ActionResult ViewQuotations()
        {
            string managerUsername = Session["Username"] as string;

            if (string.IsNullOrEmpty(managerUsername))
            {
                TempData["ErrorMessage"] = "You must be logged in to view your quotations.";
                return RedirectToAction("Login", "Login");
            }

            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Purchases").Result;
            var purchaseList = response.Content.ReadAsAsync<IEnumerable<MVCPurchasesModel>>().Result;

            var managerPurchases = purchaseList.Where(p => p.Status == "Requested" || p.Status == "Successful");

            return View(managerPurchases);
        }

        public ActionResult RequestedQuotations()
        {
            string managerUsername = Session["Username"] as string;

            if (string.IsNullOrEmpty(managerUsername))
            {
                TempData["ErrorMessage"] = "You must be logged in to view your quotations.";
                return RedirectToAction("Login", "Login");
            }

            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Purchases").Result;
            var purchaseList = response.Content.ReadAsAsync<IEnumerable<MVCPurchasesModel>>().Result;

            // Filter for Requested Quotations
            var requestedQuotations = purchaseList.Where(p => p.Status == "Requested");

            return View(requestedQuotations);
        }

        public ActionResult SuccessfulQuotations()
        {
            string managerUsername = Session["Username"] as string;

            if (string.IsNullOrEmpty(managerUsername))
            {
                TempData["ErrorMessage"] = "You must be logged in to view your quotations.";
                return RedirectToAction("Login", "Login");
            }

            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Purchases").Result;
            var purchaseList = response.Content.ReadAsAsync<IEnumerable<MVCPurchasesModel>>().Result;

            // Filter for Successful Quotations
            var successfulQuotations = purchaseList.Where(p => p.Status == "Successful");

            return View(successfulQuotations);
        }

        public ActionResult RejectedQuotations()
        {
            string managerUsername = Session["Username"] as string;

            if (string.IsNullOrEmpty(managerUsername))
            {
                TempData["ErrorMessage"] = "You must be logged in to view your quotations.";
                return RedirectToAction("Login", "Login");
            }

            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Purchases").Result;
            var purchaseList = response.Content.ReadAsAsync<IEnumerable<MVCPurchasesModel>>().Result;

            // Filter for Rejected Quotations
            var rejectedQuotations = purchaseList.Where(p => p.Status == "Rejected");

            return View(rejectedQuotations);
        }


    }
}
