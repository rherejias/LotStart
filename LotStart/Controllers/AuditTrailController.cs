using LotStart.Helpers;
using LotStart.Models;
using LotStart.ViewModels;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LotStart.Controllers
{
    public class AuditTrailController : Controller
    {
        private CustomHelper custom_helper = new CustomHelper();
        private AuditTrailModels audit_trail = new AuditTrailModels();
        private AuditTrailObject auditObj = new AuditTrailObject();

        // GET: AuditTrail
        public ActionResult AuditTrail()
        {
            try
            {
                ViewBag.Menu = custom_helper.PrepareMenu(6, 0, Session["user_type"].ToString(), Int32.Parse(Session["userId_local"].ToString()));
                ViewBag.Title = "AuditTrail";
                ViewBag.PageHeader = "AuditTrail";
                return View();
            }
            catch (Exception)
            {
                return View("~/Views/Account/login.cshtml");
            }
        }

        /// <summary>
        /// get data from local table
        /// </summary>
        /// <returns> json array</returns>
        /// rherejias 4/25/2017
        [HttpGet]
        public JsonResult GetAuditTrail()
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
            dynamic data = j.Deserialize(audit_trail.getRecord(), typeof(object));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get searched recored
        /// </summary>
        /// <returns>json array</returns>
        /// rherejias 4/25/2017
        public JsonResult GetSearch()
        {
            try
            {
                JavaScriptSerializer j = new JavaScriptSerializer();
                dynamic data = j.Deserialize(audit_trail.getSearchRecord(Request["searchParam"].ToString()), typeof(object));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return null;
            }

        }

        /// <summary>
        /// insert into audit trail table
        /// </summary>
        /// rherejias 5/9/2017
        public void doAudit()
        {
            auditObj.Module = Request["module"].ToString();
            auditObj.Operation = Request["operation"].ToString();
            auditObj.Object = Request["object"].ToString();
            auditObj.ObjectId = Request["objectId"].ToString();
            auditObj.ObjectCode = Request["objectCode"].ToString();
            auditObj.UserCode = Convert.ToInt32(Session["userId_local"].ToString());
            auditObj.IP = CustomHelper.GetLocalIPAddress();
            auditObj.MAC = CustomHelper.GetMACAddress();
            auditObj.DateAdded = DateTime.Now;

            audit_trail.Audit(auditObj);
        }
    }
}