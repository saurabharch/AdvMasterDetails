using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvMasterDetails.Models;
using System.Text;
using System.Data.Entity.Validation;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Security.Principal;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.Security;
using System.Threading.Tasks;

namespace AdvMasterDetails.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(AdvMasterDetails.Models.Login login, string ReturnUrl = "")
        {
            bool Status = false;
            string message = "";
            using (InventoryDBEntities1 dc = new InventoryDBEntities1())
            {
                if (IsEmailExist(login.Email))
                {
                    var v = dc.UserLogins.Where(a => a.Email == login.Email).FirstOrDefault();
                    if (v != null)
                    {
                        if (string.Compare(System.Web.Helpers.Crypto.Hash(login.Password), v.Password) == 0)
                        {
                            int timeout = login.RememberMe ? 525600 : 20;// 525600 minute = 1 year here timeout time is 20 min
                            var ticket = new FormsAuthenticationTicket(login.Email, login.RememberMe, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);
                            if (v.AdminType == true)
                            {
                                Status = true;
                                // return 
                                message = "Successfully Login";
                                return RedirectToAction("AprrovedOrder", "Home");
                            }
                            else if (Url.IsLocalUrl(ReturnUrl))
                            {
                                message = "Redirect To Home";
                                return Redirect(ReturnUrl);
                            }
                        }
                        else
                        {
                            Status = false;
                            message = "Invalid Credential Provided ";
                        }

                    }
                    else
                    {
                        message = "Invalid Credential Provided ";
                    }
                }
                else
                {
                        message = "Invalid Credential Provided ";
                }
            }
            ViewBag.Status = Status;
            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public ActionResult AdminReg()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminReg([Bind(Exclude = "AdminType")]AdvMasterDetails.UserLogin user)
        {
            bool Status = false;
            string message = "";
            //Model Validation
            if (ModelState.IsValid)
            {

                #region //Email is already Exist Check
                var isExist = IsEmailExist(user.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email is already Exist");
                    return View(user);
                }
                // user.UserID = user.UserID;
                #endregion
                #region Generate Activation Code
                //user. = Guid.NewGuid();
                #endregion
                #region Password Hashing
                user.Password = System.Web.Helpers.Crypto.Hash(user.Password);
                user.SecretKey = Guid.NewGuid();
                #endregion
                #region Save Data to Database
                using (InventoryDBEntities1 dc = new InventoryDBEntities1())
                {
                    dc.UserLogins.Add(user);
                    try
                    {
                        dc.SaveChanges();
                        message = "Registration is successfully done. Account activation link " +
                            " has been sent to your email id : " + user.Email;
                        Status = true;

                    }
                    catch (DbEntityValidationException ex)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                            eve.Entry.Entity.GetType().Name,
                                                            eve.Entry.State));
                            foreach (var ve in eve.ValidationErrors)
                            {
                                sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                            ve.PropertyName,
                                                            ve.ErrorMessage));
                            }
                        }
                        message = "Error Message  :" + ex;
                    }
                    //Send Email to Users
                    // return RedirectToAction("Registration", "User");

                }
                #endregion

            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }


        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (InventoryDBEntities1 de = new InventoryDBEntities1())
            {
                var EC = de.UserLogins.Where(a => a.Email == emailID).FirstOrDefault();
                return EC != null;// if not equal to null means True
            }
        }
    }
}