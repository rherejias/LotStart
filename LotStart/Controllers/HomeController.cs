using LotStart.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LotStart.Controllers
{
    public class HomeController : Controller
    {
        private CustomHelper custom_helper = new CustomHelper();
        private Dictionary<string, object> response = new Dictionary<string, object>();

        /// <summary>
        /// dashboard view
        /// </summary>
        /// <returns> view</returns>
        /// rherejias   5/9/2017
        [CustomAuthorize]
        public ActionResult Index()
        {
            ViewBag.Menu = custom_helper.PrepareMenu(1, 0, Session["user_type"].ToString(), Int32.Parse(Session["userId_local"].ToString()));
            ViewBag.Title = "Dashboard";
            ViewBag.PageHeader = "Dashboard";
            ViewBag.Breadcrumbs = "Home / Dashboard";
            return View();
        }

    }
}