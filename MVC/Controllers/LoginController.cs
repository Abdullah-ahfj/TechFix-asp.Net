using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View(new MVCLoginModel());
        }

        [HttpPost]
        public ActionResult Login(MVCLoginModel user)
        {
            string adminEmail = "admin@gmail.com";
            string adminPassword = "12345";

            if (user.Email == adminEmail && user.Password == adminPassword)
            {
                TempData["SuccessMessage"] = "Login Successful";
                Session["Username"] = "admin";  // Store admin username in session
                return RedirectToAction("Home", "Manager");
            }

            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Users").Result;
            var userList = response.Content.ReadAsAsync<IEnumerable<MVCLoginModel>>().Result;

            var loggedInUser = userList.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if (loggedInUser != null)
            {
                TempData["SuccessMessage"] = "Login Successful";
                Session["Username"] = loggedInUser.Username;  // Store the logged-in user's username in the session

                if (loggedInUser.Username == "supplier1" || loggedInUser.Username == "supplier2")
                {
                    return RedirectToAction("Home", "Supplier1");  // Redirect suppliers to the supplier page
                }
                else
                {
                    return RedirectToAction("Index", "Manager");  
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid Username or Password";
                return View(user);
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();  // Clear the session
            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Login");
        }

    }
}