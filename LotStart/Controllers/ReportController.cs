using LotStart.Helpers;
using LotStart.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LotStart.Controllers
{
    public class ReportController : Controller
    {

        private readonly CustomHelper customHelper = new CustomHelper();
        private readonly ValidateModels validateModel = new ValidateModels();
        private readonly ReportModel reportModel = new ReportModel();
        private readonly Dictionary<string, object> response = new Dictionary<string, object>();

        // GET: Reports
        public ActionResult Report()
        {
            try
            {
                ViewBag.Menu = customHelper.PrepareMenu(child: 0, parent: 5, usertype: Session["user_type"].ToString(), userId: Int32.Parse(Session["userId_local"].ToString()));
                return View();
            }
            catch (Exception)
            {
                return View(viewName: "~/Views/Account/login.cshtml");
            }
        }

        /// <summary>
        /// get all record 
        /// </summary>
        /// <returns>json</returns>
        /// avillena 4/26/2017
        public ActionResult GetReport()
        {

            try
            {
                var reportRecords = new DataTable();
                reportRecords = reportModel.GetRecordForReports();

                response.Add(key: "success", value: true);
                response.Add(key: "error", value: false);
                response.Add(key: "message", value: customHelper.DataTableToJson(reportRecords));
            }
            catch (Exception e)
            {
                response.Add(key: "success", value: false);
                response.Add(key: "error", value: true);
                response.Add(key: "message", value: e.ToString());
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get searched recored
        /// </summary>
        /// <returns>json array</returns>
        /// rherejias 4/25/2017
        public JsonResult GetSearch(string searchParam, string createdFrom, string createdTo, string requiredFrom, string requiredTo, string submittedFrom, string submittedTo)
        {
            try
            {
                JavaScriptSerializer j = new JavaScriptSerializer();
                dynamic data = j.Deserialize(reportModel.getSearchRecord(searchParam, createdFrom, createdTo, requiredFrom, requiredTo, submittedFrom, submittedTo), typeof(object));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return null;
            }

        }
    }
}