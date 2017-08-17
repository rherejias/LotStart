using LotStart.Helpers;
using LotStart.Models;
using LotStart.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Script.Serialization;


namespace LotStart.Controllers
{
    public class ValidateController : Controller
    {

        private readonly ValidateModels validate = new ValidateModels();
        private readonly MoveOrderModels moveOrderModel = new MoveOrderModels();
        private readonly AuditTrailObject auditObj = new AuditTrailObject();
        private readonly AuditTrailModels auditModels = new AuditTrailModels();
        private readonly LogsObject logsObj = new LogsObject();
        private readonly CustomHelper customHelper = new CustomHelper();
        private readonly ValidateModels validateModel = new ValidateModels();
        private readonly Dictionary<string, object> response = new Dictionary<string, object>();
        private readonly ItemObject itemObj = new ItemObject();
        readonly DateTime now = DateTime.Now;

        // GET: Validate

        /// <summary>
        /// for validation view
        /// </summary>
        /// <returns>view</returns>
        /// author rherejias 4/20/2017
        public ActionResult Validate()
        {
            try
            {
                if (Request["from"].ToString() == "0")
                {
                    ViewBag.hiddenFrom = "1";
                }
                else
                {
                    ViewBag.moveOrder = Request["moveOrder"].ToString();
                    ViewBag.subInventory = Request["subInventory"].ToString();
                    ViewBag.planner = Request["planner"].ToString();
                    ViewBag.dateCreated = Request["dateCreated"].ToString();
                    ViewBag.dateRequired = Request["dateRequired"].ToString();
                    ViewBag.status = Request["Status"].ToString();
                    ViewBag.package = Request["Package"].ToString();

                    ViewBag.Item = Request["Item"].ToString();
                    ViewBag.Org = Request["Org"].ToString();
                    ViewBag.TargetCAS = Request["TargetCAS"].ToString();
                    ViewBag.Quantity = Request["Quantity"].ToString();
                }
                ViewBag.Menu = customHelper.PrepareMenu(4, 0, Session["user_type"].ToString(), Int32.Parse(Session["userId_local"].ToString()));
                return View();
            }
            catch (Exception e)
            {
                return View(viewName: "~/Views/Account/login.cshtml");
            }
        }

        /// <summary>
        /// to retain search filters
        /// </summary>
        /// <returns>view</returns>
        /// author rherejias 4/20/2017
        public void SearchFilters()
        {
            try
            {
                FIlterObject.moveOrder = Request["moveOrder"].ToString();
                FIlterObject.item = Request["item"].ToString();
                FIlterObject.planner = Request["planner"].ToString();
                FIlterObject.package = Request["pkg"].ToString();
                FIlterObject.createdFrom = Request["createdFrom"].ToString();
                FIlterObject.createdTo = Request["createdTo"].ToString();
                FIlterObject.requiredFrom = Request["requiredFrom"].ToString();
                FIlterObject.requiredTo = Request["requiredTo"].ToString();

            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// get item on first load
        /// </summary>
        /// <returns>json</returns>
        ///  author rherejias 5/8/2017
        public JsonResult GetItem()

        {
            try
            {
                var j = new JavaScriptSerializer();
                dynamic data = j.Deserialize(validateModel.GetRecord(Request["moveOrder"].ToString()), typeof(object));


                return Json(data, JsonRequestBehavior.AllowGet);


            }
            catch (Exception)
            {

                return null;
            }

        }

        /// <summary>
        /// submit validated move order 
        /// </summary>
        /// <returns>view to move order </returns>
        ///  author rherejias 5/8/2017
        public void Submit()
        {
            //audit
            auditObj.Module = "VALIDATE";
            auditObj.Operation = "SUBMIT";
            auditObj.Object = "tblLogs";
            auditObj.ObjectId = Request["objectId"].ToString();
            auditObj.ObjectCode = Request["objectCode"].ToString();
            auditObj.UserCode = Convert.ToInt32(Session["userId_local"].ToString());
            auditObj.IP = CustomHelper.GetLocalIPAddress();
            auditObj.MAC = CustomHelper.GetMACAddress();
            auditObj.DateAdded = now;

            auditModels.Audit(auditObj);

            string[] lot = Request["lotNumber"].ToString().Split(separator: ',');

            for (int i = 0; i < lot.Length; i++)
            {
                logsObj.MoveOrderNbr = Request["objectCode"].ToString();
                logsObj.Org = Request["org"].ToString();
                logsObj.Item = Request["item"].ToString();
                logsObj.LotNumber = lot[i];
                logsObj.TargetCAS = Request["targetCAS"].ToString();
                logsObj.Planner = Request["planner"].ToString();
                logsObj.Quantity = Request["quantity"].ToString();
                logsObj.DateCreated = Convert.ToDateTime(Request["dateCreated"].ToString());
                logsObj.DateRequired = Convert.ToDateTime(Request["dateRequired"].ToString());
                logsObj.SubInventory = Request["subInventory"].ToString();
                logsObj.Package = Request["pkg"].ToString();
                logsObj.User = Convert.ToInt32(Session["userId_local"].ToString());
                logsObj.DateSubmitted = now;
                logsObj.Status = "Validated";

                auditModels.Logs(logsObj);

                itemObj.MoveOrder = Request["objectCode"].ToString();
                itemObj.Lot = lot[i];
                itemObj.Quantity = Request["quantity"].ToString();
                itemObj.Locator = Request["locator"].ToString();

                auditModels.ItemLogs(itemObj);
            }

        }
        /// <summary>
        /// author : avillena | desc : to get the move order lot details | date : 04/25/2017 | version : 1.0
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSearchedMoveOrderValidatePage()
        {
            try
            {
                var j = new JavaScriptSerializer();
                dynamic data = j.Deserialize(validateModel.GetRecordofLotDetails(Request["moveOrder"].ToString()), typeof(object));

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return null;
            }

        }

        // author : avillena | desc : send email notification for move order ready for pick up | info : triggered by window sevices (Rikimaro.exe) every 15 mins | date : 5/3/2017 | version : 1.0
        public JsonResult ReadMoveOrders()
        {
            try
            {
                dynamic data;

                try
                {
                    var j = new JavaScriptSerializer();
                    data = j.Deserialize(validateModel.GetRecordofTestStage_ForEmail(), typeof(object));

                }
                catch (Exception)
                {

                    customHelper.FailedJobs(err: "Failed to send : Available Move Order Email Notifcation | Cause : No record found / Oracle query failed");
                    return Json(ReadMoveOrdersForWarehouse(), JsonRequestBehavior.AllowGet);
                }

                // if condition return null to do nothing and send email notif if no record on data param

                string emailContent = "<table style='width:95%;font-size:14; font-family:Arial; border: 1px solid #0e5866; border-collapse: collapse; font-size:12;' cellpadding='7' align='center'>" +

                                     "<tr>" +
                                     "<td style='border-color:white' colspan='8'><strong>Please be informed that the following move order/s is/are ready for pick up :</strong></td>" +
                                     "</tr>" +
                                     "<tr>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Org</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Date Required</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Package Code</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Move Order</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Item</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Rev</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Target CAS</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Lot Number</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Quantity</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Date Transacted</strong></td>" +
                                          "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Last Updated By</strong></td>" +
                                     "</tr>";


                foreach (var details in data)

                {
                    string rowcolorOverdue = "";


                    // DateTime newDate = DateTime.Now.AddHours(Int32.Parse(ConfigurationManager.AppSettings["hoursdurationofoverdue"].ToString()));
                    if (Convert.ToDateTime(details["DATEREQUIRED"]) < DateTime.Now)
                    {
                        rowcolorOverdue = "red";
                    }
                    else
                    {
                        rowcolorOverdue = "black";
                    }
                    emailContent += "<tr style='color:" + rowcolorOverdue + "'>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["ORG"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["DATEREQUIRED"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["PACKAGE_CODE"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["MOVEORDERNBR"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["ITEM"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["REV"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["CAS_ITEM"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["LOT_NUMBER"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["QUANTITY"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["DATE_TRANSACTED"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["LAST_UPDATED_BY"].ToString() + "</td>" +
                                     "</tr>";



                }
                emailContent += "</table>&nbsp;&nbsp;";


                List<List<string>> emailAddress = customHelper.GetEmailAddressforRecipient();
                List<string> emailAddressList = emailAddress[0];

                // email address into list
                var emailListedReady = new List<string>();
                for (int i = 0; i < emailAddressList.Count; i++)
                {
                    emailListedReady.Add(emailAddressList[i]);
                }

                // from list into string separated by ","
                string emaailAddressListed = string.Join(separator: ",", values: emailListedReady);

                string sender = ConfigurationManager.AppSettings["email_sender"].ToString();

                using (var message = new MailMessage(sender, to: emaailAddressListed))
                {
                    message.Subject = ConfigurationManager.AppSettings["email_subject"].ToString();
                    message.Body = customHelper.EmailTemplate(emailContent);
                    message.IsBodyHtml = true;
                    message.Bcc.Add(addresses: ConfigurationManager.AppSettings["email_bcc"].ToString());
                    using (SmtpClient client = new SmtpClient
                    {
                        EnableSsl = false,
                        Host = ConfigurationManager.AppSettings["outlook_host"].ToString(),
                        Port = 25,
                        Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ampinoreply_email_add"].ToString(), ConfigurationManager.AppSettings["ampinoreply_password"].ToString())
                    })
                    {
                        client.Send(message);
                    }
                }

                // call ReadMoveOrdersForWarehouse function for separate email notification
                ReadMoveOrdersForWarehouse();
                //end
                response.Add(key: "success", value: true);
                response.Add(key: "error", value: false);
                response.Add(key: "message", value: true);


            }
            catch (Exception e)
            {
                response.Add(key: "success", value: false);
                response.Add(key: "error", value: true);
                response.Add(key: "message", value: e.ToString());

            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        // author : avillena | desc : send email notification for move order ready for pick up for warehouse | info : triggered by window sevices (Rikimaro.exe) every 15 mins | date : 5/3/2017 | version : 1.0
        public JsonResult ReadMoveOrdersForWarehouse()
        {
            try
            {
                dynamic dataforWarehouse;
                try
                {
                    var j1 = new JavaScriptSerializer();
                    dataforWarehouse = j1.Deserialize(validateModel.GetRecordFromOracle_ForEmailWareHouse(), typeof(object));
                }
                catch (Exception)
                {
                    customHelper.FailedJobs(err: "Failed to send : Warehouse Email Notifcation | Cause : No record found / Oracle query failed");
                    return null;
                }
                // if condition return null to do nothing and send email notif if no record on data param
                string emailContentforWarehouser = "<table style='width:95%;font-size:14; font-family:Arial; border: 1px solid #0e5866; border-collapse: collapse; font-size:12;' cellpadding='7' align='center'>" +

                                     "<tr>" +
                                     "<td style='border-color:white' colspan='8'><strong>Please be informed that the following move order/s is/are ready for staging :</strong></td>" +
                                     "</tr>" +
                                     "<tr>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Org</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Date Required</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Package Code</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Move Order</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Item</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Rev</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Target CAS</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Lot Number</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Quantity</strong></td>" +
                                         "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Last Updated</strong></td>" +
                                          "<td style='border: 1px solid #0e5866; border-collapse: collapse; background-color:#0e5866; color:#ffffff'><strong>Last Updated By</strong></td>" +
                                     "</tr>";


                foreach (var details in dataforWarehouse)

                {
                    string rowcolorOverdue = "";


                    // DateTime newDate = DateTime.Now.AddHours(Int32.Parse(ConfigurationManager.AppSettings["hoursdurationofoverdue"].ToString()));
                    if (Convert.ToDateTime(details["DATEREQUIRED"]) < DateTime.Now)
                    {
                        rowcolorOverdue = "red";
                    }
                    else
                    {
                        rowcolorOverdue = "black";
                    }
                    emailContentforWarehouser += "<tr style='color:" + rowcolorOverdue + "'>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["ORG"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["DATEREQUIRED"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["PACKAGE_CODE"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["MOVEORDERNBR"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["SOURCEITEM"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["SOURCEREV"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["CAS_ITEM"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["LOT_NUMBER"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["QUANTITY"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["LASTUPDATED"].ToString() + "</td>" +
                                        "<td style='border: 1px solid #0e5866; border-collapse: collapse;'>" + details["LAST_UPDATED_BY"].ToString() + "</td>" +
                                     "</tr>";



                }
                emailContentforWarehouser += "</table>&nbsp;&nbsp;";


                List<List<string>> emailAddressWarehouse = customHelper.GetEmailAddressforRecipientforWarehouse();
                List<string> emailAddressListWarehouse = emailAddressWarehouse[0];

                // email address into list
                var emailListedReadyWare = new List<string>();
                for (int i = 0; i < emailAddressListWarehouse.Count; i++)
                {
                    emailListedReadyWare.Add(emailAddressListWarehouse[i]);
                }

                // from list into string separated by ","
                string emaailAddressListedWare = string.Join(separator: ",", values: emailListedReadyWare);

                string sender = ConfigurationManager.AppSettings["email_sender"].ToString();

                using (var message = new MailMessage(sender, to: emaailAddressListedWare))
                {
                    message.Subject = ConfigurationManager.AppSettings["email_subject_for_Warehouse"].ToString();
                    message.Body = customHelper.EmailTemplate(emailContentforWarehouser);
                    message.IsBodyHtml = true;
                    message.Bcc.Add(addresses: ConfigurationManager.AppSettings["email_bcc"].ToString());
                    using (SmtpClient client = new SmtpClient
                    {
                        EnableSsl = false,
                        Host = ConfigurationManager.AppSettings["outlook_host"].ToString(),
                        Port = 25,
                        Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ampinoreply_email_add"].ToString(), ConfigurationManager.AppSettings["ampinoreply_password"].ToString())
                    })
                    {
                        client.Send(message);
                    }
                }
            }
            catch (Exception e)
            {
                response.Add(key: "success", value: false);
                response.Add(key: "error", value: true);
                response.Add(key: "message", value: e.ToString());
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }


    }
}