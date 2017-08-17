using LotStart.Models;
using LotStart.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace LotStart.Controllers
{
    public class AccountController : Controller
    {
        private UserModels userModel = new UserModels();
        private AuditTrailObject auditObj = new AuditTrailObject();
        private AuditTrailModels auditModels = new AuditTrailModels();
        private UserObject userObj = new UserObject();

        private Dictionary<string, object> response = new Dictionary<string, object>();
        private Dictionary<string, object> r = new Dictionary<string, object>();

        /// <summary>
        /// author: rherejias@allegromicro.com
        /// description : user log in view
        /// version : 1.0
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (returnUrl == "/" || returnUrl == "")
            {
                returnUrl =
                    ConfigurationManager.AppSettings[
                        ConfigurationManager.AppSettings["env"].ToString() + "_base_url"].ToString();
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        /// <summary>
        /// author: rherejias@allegromicro.com
        /// description : user log in and validation , get the log in details to AD server
        /// version : 1.0
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        [AllowAnonymous]
        [HttpPost]
        public JsonResult Attempt(string username, string password, string returnurl, bool isauto)
        {
            try
            {
                string userType = "";
                if (ModelState.IsValid)
                {
                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ api login
                    Library.EncryptDecrypt.EncryptDecryptPassword e = new Library.EncryptDecrypt.EncryptDecryptPassword();

                    HttpClientHandler hndlr = new HttpClientHandler();
                    hndlr.UseDefaultCredentials = true;

                    HttpClient client = new HttpClient(hndlr);
                    //client.BaseAddress = new Uri("http://localhost:52889/");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string pw = password;

                    if (!isauto)
                    {
                        pw = Server.UrlEncode(e.EncryptPassword(password));
                    }

                    var url = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["env"].ToString() + "_api_base_url"].ToString() + "login/Authenticate?username=" + username + "&password=" + pw + "&json=true";
                    HttpResponseMessage res = client.GetAsync(url).Result;

                    if (res.IsSuccessStatusCode)
                    {
                        string strJson = res.Content.ReadAsStringAsync().Result;
                        dynamic jObj = (JObject)JsonConvert.DeserializeObject(strJson);

                        JavaScriptSerializer j = new JavaScriptSerializer();
                        object a = j.Deserialize(strJson, typeof(object));

                        Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(strJson);

                        if (bool.Parse(dict["success"].ToString()))
                        {
                            //url for getting user info from AD
                            url = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["env"].ToString() + "_api_base_url"].ToString() + "login/userinfo?username=" + username + "&json=true";
                            res = client.GetAsync(url).Result;
                            if (res.IsSuccessStatusCode)
                            {
                                strJson = res.Content.ReadAsStringAsync().Result;
                                jObj = (JObject)JsonConvert.DeserializeObject(strJson);
                                a = j.Deserialize(strJson, typeof(object));
                                dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(strJson);

                                ADUserObject userObject = new ADUserObject();
                                try { userObject.Username = dict["sAMAccountName"].ToString(); }
                                catch (Exception) { userObject.Username = ""; }

                                try { userObject.LastName = dict["sn"].ToString(); }
                                catch (Exception) { userObject.LastName = ""; }
                                try { userObject.GivenName = dict["givenName"].ToString(); }
                                catch (Exception) { userObject.GivenName = ""; }
                                try { userObject.EmployeeNbr = dict["employeeNumber"].ToString(); }
                                catch (Exception) { userObject.EmployeeNbr = ""; }
                                try { userObject.Email = dict["mail"].ToString(); }
                                catch (Exception) { userObject.Email = ""; }
                                try { userObject.Department = dict["department"].ToString(); }
                                catch (Exception) { userObject.Department = ""; }
                                try { userObject.ThumbnailPhoto = dict["thumbnailPhoto"].ToString(); }
                                catch (Exception) { userObject.ThumbnailPhoto = ""; }
                                userObject.IsActive = true;
                                userObject.AddedBy = 0;
                                userObject.DateAdded = DateTime.Now;
                                userObject.Source = "AD";


                                int IsInLocal = userModel.CheckIdFromLocal(dict["mail"].ToString());
                                //int IsInLocal = 0;
                                if (IsInLocal <= 0)
                                {
                                    userObject.Type = "user";
                                    IsInLocal = userModel.AddUser(userObject);
                                    userType = userModel.GetUserType(IsInLocal);
                                }
                                else
                                {
                                    userType = userModel.GetUserType(IsInLocal);

                                    userObject.Id = IsInLocal;
                                    userObject.Type = userType;

                                    IsInLocal = userModel.UpdateUser(userObject);
                                }


                                HttpContext.Session.Add("username", username);
                                HttpContext.Session.Add("loggedIn", true);
                                HttpContext.Session.Add("userId_local", IsInLocal);
                                HttpContext.Session.Add("user_type", userType);
                                //rherejias added try catch to all AD information retrieved by Login_API
                                try { HttpContext.Session.Add(name: "cn", value: dict["cn"]); } catch { HttpContext.Session.Add(name: "cn", value: ""); }
                                try { HttpContext.Session.Add(name: "title", value: dict["title"]); } catch { HttpContext.Session.Add(name: "title", value: ""); }
                                try { HttpContext.Session.Add(name: "department", value: dict["department"]); } catch { HttpContext.Session.Add(name: "department", value: ""); }
                                try { HttpContext.Session.Add(name: "company", value: dict["company"]); } catch { HttpContext.Session.Add(name: "company", value: ""); }
                                try { HttpContext.Session.Add(name: "employeeNumber", value: dict["employeeNumber"]); } catch { HttpContext.Session.Add(name: "employeeNumber", value: ""); }
                                try { HttpContext.Session.Add(name: "mail", value: dict["mail"]); } catch { HttpContext.Session.Add(name: "mail", value: ""); }
                                try { HttpContext.Session.Add(name: "thumbnailPhoto", value: dict["thumbnailPhoto"]); } catch { HttpContext.Session.Add(name: "thumbnailPhoto", value: ""); }
                            }

                            r.Add("success", true);
                            r.Add("error", false);

                        }
                    }
                    else
                    {
                        throw new Exception(res.IsSuccessStatusCode.ToString());
                        //response.Add("message", "An error occured! Please check the api endpoint for <login/Authenticate>.");
                    }
                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ api login
                    var ac = r["success"];
                    if (ac.ToString() == "True")
                    {
                        //Session["loggedIn"] = "True";
                        //Session.Add("loggedIn", true);
                        HttpContext.Session.Add("loggedIn", true);

                        FormsAuthentication.SetAuthCookie(username, true);

                        //set account credentials into browser cookie
                        var logInCookie = new HttpCookie(name: "logInCookie");
                        logInCookie.Values["UName"] = username;
                        logInCookie.Values["PWord"] = pw;
                        logInCookie.Values["lastVisit"] = DateTime.Now.ToString();
                        logInCookie.Expires = DateTime.Now.AddDays(value: 30);
                        Response.Cookies.Add(logInCookie);

                        //return Redirect(returnUrl);
                        if (returnurl == "" || returnurl == null || returnurl == "/")
                        {
                            response.Add("message", ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["env"].ToString() + "_base_url"].ToString());
                        }
                        else
                        {
                            response.Add("message", returnurl);
                        }
                        //put audit trail here
                        //var obj = Deserialize<List<UserObject>>(auditModels.getUserData(username));
                        //string test = obj[0].Id.ToString();

                        auditObj.Module = "ACCOUNT";
                        auditObj.Operation = "LOGIN";
                        auditObj.Object = "tblUsers";
                        auditObj.ObjectId = auditModels.getUserID(username);
                        auditObj.ObjectCode = auditModels.getUserCode(username).ToString();
                        auditObj.UserCode = Convert.ToInt32(auditModels.getUserID(username));
                        auditObj.IP = Helpers.CustomHelper.GetLocalIPAddress();
                        auditObj.MAC = Helpers.CustomHelper.GetMACAddress();
                        auditObj.DateAdded = DateTime.Now;

                        auditModels.Audit(auditObj);

                        FIlterObject.moveOrder = "";
                        FIlterObject.item = "";
                        FIlterObject.planner = "";
                        FIlterObject.package = "";
                        FIlterObject.createdFrom = "";
                        FIlterObject.createdTo = "";
                        FIlterObject.requiredFrom = "";
                        FIlterObject.requiredTo = "";

                        response.Add("success", true);
                        response.Add("error", false);

                    }
                    else
                    {
                        throw new Exception("Page return: " + res.IsSuccessStatusCode);
                    }
                }
                else
                {
                    throw new Exception("Login failed!");
                }
            }
            catch (Exception e)
            {
                response.Add("success", false);
                response.Add("error", true);
                if (e.ToString().IndexOf("The supplied credential is invalid") != -1)
                {
                    response.Add("message", "Username and/or password is incorrect!");
                    HttpCookie logInCookie = new HttpCookie("logInCookie");
                    logInCookie.Expires = DateTime.Now.AddDays(-1);
                }
                else
                {
                    HttpCookie logInCookie = new HttpCookie("logInCookie");
                    response.Add("message", "Username and/or password is incorrect!");
                    // return Redirect(returnUrl);
                    logInCookie.Expires = DateTime.Now.AddDays(-1);


                }
            }


            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// author: rherejias@allegromicro.com
        /// description : user log out
        /// version : 1.0
        /// </summary>
        /// <returns></returns>
        public JsonResult Logout()
        {
            Session.Remove("userId_local");
            Session.Remove("cn");
            Session.Remove("title");
            Session.Remove("department");
            Session.Remove("company");
            Session.Remove("employeeNumber");
            Session.Remove("mail");
            Session.Remove("thumbnailPhoto");
            Session.Remove("loggedIn");

            response.Add("success", true);
            response.Add("error", false);
            response.Add("message", "");

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// author: rherejias@allegromicro.com
        /// description : validate add new user to local tbluser
        /// version : 1.0
        /// </summary>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// author: rherejias@allegromicro.com
        /// description : validate add new user to local tbluser
        /// version : 1.0
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Validate()

        {
            try
            {
                string username = Request["username"].ToString();
                string adduser = Request["adduser"].ToString();
                HttpClientHandler hndlr = new HttpClientHandler();
                hndlr.UseDefaultCredentials = true;

                HttpClient client = new HttpClient(hndlr);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                string url;
                url = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["env"].ToString() + "_api_base_url"].ToString() + "login/userinfo?username=" + username + "&json=true";
                HttpResponseMessage res = client.GetAsync(url).Result;

                if (res.IsSuccessStatusCode)
                {
                    string strJson = res.Content.ReadAsStringAsync().Result;
                    dynamic jObj = (JObject)JsonConvert.DeserializeObject(strJson);

                    JavaScriptSerializer j = new JavaScriptSerializer();
                    object a = j.Deserialize(strJson, typeof(object));

                    Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(strJson);

                    if (dict.Count > 0)
                    {
                        //url for getting user info from AD

                        res = new HttpResponseMessage();
                        res = client.GetAsync(url).Result;
                        if (res.IsSuccessStatusCode)
                        {
                            strJson = res.Content.ReadAsStringAsync().Result;
                            jObj = (JObject)JsonConvert.DeserializeObject(strJson);
                            a = j.Deserialize(strJson, typeof(object));
                            dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(strJson);


                            ADUserObject userObject = new ADUserObject();
                            userObject.Username = dict["sAMAccountName"].ToString();
                            userObject.LastName = dict["sn"].ToString();
                            userObject.GivenName = dict["givenName"].ToString();
                            userObject.EmployeeNbr = dict["employeeNumber"].ToString();
                            userObject.Email = dict["mail"].ToString();
                            userObject.IsActive = true;
                            userObject.Department = dict["department"].ToString();
                            userObject.AddedBy = 0;
                            userObject.DateAdded = DateTime.Now;
                            userObject.Source = "AD";
                            userObject.Type = "user";


                            try
                            {
                                string image;
                                image = dict["thumbnailPhoto"].ToString();
                                userObject.ThumbnailPhoto = image;
                            }
                            catch
                            {
                                userObject.ThumbnailPhoto = null;

                            }


                            if (adduser == "YES")
                            {

                                int usernameCount = userModel.Username_count(userObject.Username);

                                if (usernameCount == 0)
                                {
                                    userModel.AddUser(userObject);
                                    response.Add(key: "success", value: true);
                                    response.Add("error", false);
                                    response.Add("message", "Added succesfully");

                                    return Json(response, JsonRequestBehavior.AllowGet);

                                }
                                else
                                {

                                    response.Add("success", false);
                                    response.Add("error", true);
                                    response.Add("message", "Username has already been added!");

                                    return Json(response, JsonRequestBehavior.AllowGet);
                                }


                            }


                            response.Add("success", true);
                            response.Add("error", false);
                            response.Add("message", userObject);

                        }



                    }
                    else
                    {

                        throw new Exception("Invalid Username!");

                    }



                }


            }
            catch (Exception e)
            {
                response.Add("success", false);
                response.Add("error", true);
                response.Add("message", "Invalid Username!");

            }


            return Json(response, JsonRequestBehavior.AllowGet);

        }
    }
}