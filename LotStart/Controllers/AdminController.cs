using LotStart.Helpers;
using System;
using System.Web.Mvc;

namespace LotStart.Controllers
{
    public class AdminController : Controller
    {
        private CustomHelper custom_helper = new CustomHelper();
        // GET: Admin

        /// <summary>
        /// author: rherejias@allegromicro.com
        /// description : Admin Access Rights Maintenance View
        /// version : 1.0
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageAccessRights()
        {
            try
            {
                ViewBag.Menu = custom_helper.PrepareMenu(2, 0, Session["user_type"].ToString(), Int32.Parse(Session["userId_local"].ToString()));
                ViewBag.Title = "Manage Access Rights";
                ViewBag.PageHeader = "Manage Access Rights";
                ViewBag.Breadcrumbs = "Admin / Manage Access Rights";
                return View();
            }
            catch (Exception)
            {
                return View("~/Views/Account/login.cshtml");
            }

        }
    }
}