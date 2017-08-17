using LotStart.Helpers;
using LotStart.Models;
using LotStart.ViewModels;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LotStart.Controllers
{
    public class ReportOracleController : Controller
    {
        private CustomHelper custom_helper = new CustomHelper();
        private ReportOracleModels ReportOraclerModel = new ReportOracleModels();
        private AuditTrailObject auditObj = new AuditTrailObject();
        private AuditTrailModels auditModels = new AuditTrailModels();

        // GET: ReportOracle
        public ActionResult Oracle()
        {
            try
            {
                ViewBag.Menu = custom_helper.PrepareMenu(child: 0, parent: 5, usertype: Session["user_type"].ToString(), userId: Int32.Parse(Session["userId_local"].ToString()));
                return View();
            }
            catch (Exception)
            {
                return View(viewName: "~/Views/Account/login.cshtml");
            }
        }

        /// <summary>
        /// get data from oracle table
        /// </summary>
        /// <returns> json string</returns>
        ///  author rherejias 5/8/2017
        [HttpGet]
        public JsonResult GetMoveOrder()
        {
            try
            {
                JavaScriptSerializer j = new JavaScriptSerializer();
                dynamic data = j.Deserialize(ReportOraclerModel.getRecord("", "", "", "", "", "", "", "", "", ""), typeof(object));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return null;
            }

        }

        /// <summary>
        /// get searched data from oracle table
        /// </summary>
        /// <returns> json string</returns>
        ///  author rherejias 5/8/2017
        [HttpGet]
        public JsonResult GetSearchedMoveOrder(string moveOrder, string item, string planner, string status,
                                               string createdFrom, string createdTo, string requiredFrom, string requiredTo, string pkg)
        {
            try
            {
                auditObj.Module = "MOVE ORDER";
                auditObj.Operation = "SEARCH";
                auditObj.Object = "OracleTable";
                auditObj.ObjectId = "";
                auditObj.ObjectCode = "";
                auditObj.UserCode = Convert.ToInt32(Session["userId_local"].ToString());
                auditObj.IP = CustomHelper.GetLocalIPAddress();
                auditObj.MAC = CustomHelper.GetMACAddress();
                auditObj.DateAdded = DateTime.Now;

                auditModels.Audit(auditObj);

                JavaScriptSerializer j = new JavaScriptSerializer();
                dynamic data = j.Deserialize(ReportOraclerModel.getRecord(moveOrder, item, planner, status, requiredFrom, requiredTo, createdFrom, createdTo, pkg, "1"), typeof(object));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return null;
            }

        }
    }
}