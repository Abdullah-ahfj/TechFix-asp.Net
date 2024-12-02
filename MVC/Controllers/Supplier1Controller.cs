using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class Supplier1Controller : Controller
    {
        // GET: Supplier1

        public ActionResult Home()
        {
            // Check if the user is logged in
            if (Session["Username"] == null)
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Login");
            }

            // Retrieve the username from session
            string supplierUsername = Session["Username"] as string;

            // Fetch all products
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Products").Result;
            var productList = response.Content.ReadAsAsync<IEnumerable<MVCSupplier1Model>>().Result;

            // Filter products by the logged-in supplier's username
            var supplierProducts = productList.Where(p => p.Username == supplierUsername);

            return View(supplierProducts);
        }


        public ActionResult Index()
        {
            // Check if the user is logged in
            if (Session["Username"] == null)
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Login");
            }

            // Retrieve the username from session
            string supplierUsername = Session["Username"] as string;

            // Fetch all products
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Products").Result;
            var productList = response.Content.ReadAsAsync<IEnumerable<MVCSupplier1Model>>().Result;

            // Filter products by the logged-in supplier's username
            var supplierProducts = productList.Where(p => p.Username == supplierUsername);

            return View(supplierProducts);
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new MVCSupplier1Model());
            }
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Products/" + id.ToString()).Result;
                return View(response.Content.ReadAsAsync<MVCSupplier1Model>().Result);
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(MVCSupplier1Model sup)
        {
            // Set the supplier's username from the session before saving
            sup.Username = Session["Username"] as string;

            if (sup.ProductId == 0)
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("Products", sup).Result;
                TempData["SuccessMessage"] = "Saved Successfully";
            }
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PutAsJsonAsync("Products/" + sup.ProductId, sup).Result;
                TempData["SuccessMessage"] = "Product Updated Successfully";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            HttpResponseMessage response = GlobalVariables.WebApiClient.DeleteAsync("Products/" + id.ToString()).Result;
            TempData["SuccessMessage"] = "Deleted Successfully";
            return RedirectToAction("Index");
        }

        public ActionResult ViewQuotations()
        {
            // Retrieve the username from session
            string supplierUsername = Session["Username"] as string;

            if (string.IsNullOrEmpty(supplierUsername))
            {
                TempData["ErrorMessage"] = "You must be logged in as a supplier to view quotations.";
                return RedirectToAction("Login", "Login");
            }

            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Purchases").Result;
            var purchaseList = response.Content.ReadAsAsync<IEnumerable<MVCPurchasesModel>>().Result;

            // Filter the purchases by the logged-in supplier's username
            var supplierPurchases = purchaseList.Where(p => p.Suppliername == supplierUsername && p.Status == "Requested");

            return View(supplierPurchases);
        }

        public ActionResult ApprovePurchase(int purchaseId)
        {
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Purchases/" + purchaseId.ToString()).Result;
            var purchase = response.Content.ReadAsAsync<MVCPurchasesModel>().Result;

            if (purchase != null)
            {
                purchase.Status = "Successful";  // Mark as successful
                HttpResponseMessage updateResponse = GlobalVariables.WebApiClient.PutAsJsonAsync("Purchases/" + purchaseId, purchase).Result;

                if (updateResponse.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Purchase approved successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error approving purchase.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Purchase not found.";
            }

            return RedirectToAction("ViewQuotations");  // Redirect to the quotations list
        }

        public ActionResult RejectPurchase(int purchaseId)
        {
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Purchases/" + purchaseId.ToString()).Result;
            var purchase = response.Content.ReadAsAsync<MVCPurchasesModel>().Result;

            if (purchase != null)
            {
                purchase.Status = "Rejected";  // Mark as rejected
                HttpResponseMessage updateResponse = GlobalVariables.WebApiClient.PutAsJsonAsync("Purchases/" + purchaseId, purchase).Result;

                if (updateResponse.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Purchase rejected successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error rejecting purchase.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Purchase not found.";
            }

            return RedirectToAction("ViewQuotations");  // Redirect to the quotations list
        }
    }
}
