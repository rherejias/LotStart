using LotStart.Helpers;
using LotStart.Models;
using LotStart.ViewModels;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LotStart.Controllers
{
    public class MoveOrderController : Controller
    {
        private CustomHelper custom_helper = new CustomHelper();
        private MoveOrderModels MoveOrderModel = new MoveOrderModels();
        private AuditTrailObject auditObj = new AuditTrailObject();
        private AuditTrailModels auditModels = new AuditTrailModels();

        // GET: MoveOrder
        /// <summary>
        /// call moveorder view
        /// </summary>
        /// <returns>view</returns>
        /// author rherejias 5/8/2017
        public ActionResult MoveOrder(string from)
        {
            try
            {
                if (from == "0")
                {
                    FIlterObject.moveOrder = "";
                    FIlterObject.item = "";
                    FIlterObject.planner = "";
                    FIlterObject.package = "";
                    FIlterObject.createdFrom = "";
                    FIlterObject.createdTo = "";
                    FIlterObject.requiredFrom = "";
                    FIlterObject.requiredTo = "";
                }

                ViewBag.Menu = custom_helper.PrepareMenu(3, 0, Session["user_type"].ToString(), Int32.Parse(Session["userId_local"].ToString()));
                ViewBag.Title = "Move Order";
                ViewBag.PageHeader = "Move Order";
                return View();
            }
            catch (Exception)
            {
                return View("~/Views/Account/login.cshtml");
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
                dynamic data = j.Deserialize(MoveOrderModel.getRecord(FIlterObject.moveOrder, FIlterObject.item, FIlterObject.planner, FIlterObject.requiredFrom,
                                        FIlterObject.requiredTo, FIlterObject.createdFrom, FIlterObject.createdTo, FIlterObject.package, ""), typeof(object));
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
        public JsonResult GetSearchedMoveOrder(string moveOrder, string item, string planner, string createdFrom, string createdTo, string requiredFrom, string requiredTo, string pkg)
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
                dynamic data = j.Deserialize(MoveOrderModel.getRecord(moveOrder, item, planner, requiredFrom, requiredTo, createdFrom, createdTo, pkg, "1"), typeof(object));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return null;
            }

        }
    }
}