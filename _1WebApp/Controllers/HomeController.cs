using FlakyTestWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FlakyTestWebApp.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Setting()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCustomerDetails_API()
        {
            //Load from database
            var customers = db.Customers.Include("Orders")
                        .Select(c => new
                        {
                            c.Id,
                            c.Name,
                            c.Email,
                            Orders = c.Orders.Select(o => new
                            {
                                o.OrderId,
                                o.Product,
                                o.Amount,
                                o.Date
                            })
                        }).ToList();
            try
            {
                return Json(customers, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            //Dummy data
            #region
            //try
            //{
            //    var customers = new List<object>();

            //    for (int i = 1; i <= 500; i++)
            //    {
            //        var orders = new List<object>();
            //        for (int j = 1; j <= 5; j++)
            //        {
            //            orders.Add(new
            //            {
            //                OrderId = (i - 1) * 5 + j,
            //                Product = $"Product {j}",
            //                Amount = 10.00m + j,
            //                Date = DateTime.Today.AddDays(-j)
            //            });
            //        }

            //        customers.Add(new
            //        {
            //            Id = i,
            //            Name = $"Customer {i}",
            //            Email = $"customer{i}@example.com",
            //            Orders = orders
            //        });
            //    }

            //    return Json(customers, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    Response.StatusCode = 500;
            //    return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            //}
            #endregion
        }

        public ActionResult UserRegistration()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UserRegister_API(string name, string email)
        {
            if (new Random().Next(0, 5) == 2)
            {
                Thread.Sleep(3000);
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        public ActionResult Feature()
        {
            return View();
        }

        public ActionResult Help()
        {
            return View();
        }
    }
}