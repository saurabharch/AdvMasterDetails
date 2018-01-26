using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvMasterDetails.Models;
using System.Net.Mail;
using System.Net;
using System.Web.Security;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.Entity.Validation;

namespace AdvMasterDetails.Controllers
{
    public class PassResetController : Controller
    {
        // GET: PassReset
        [HttpGet]
        public ActionResult Forget()
        {
            return View();
        }


        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (InventoryDBEntities de = new InventoryDBEntities())
            {
                var EC = de.UserLogins.Where(a => a.Email == emailID).FirstOrDefault();
                return EC != null;// if not equal to null means True
            }
        }
        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgetPassChange(string id)
        {
            bool Status = false;
            string message = "";
            try
            {
                using (InventoryDBEntities dc = new InventoryDBEntities())
                {
                    dc.Configuration.ValidateOnSaveEnabled = false; // Avoid Confirmation password does not match on save changes
                    var v = dc.UserLogins.Where(a => a.GlobalID == new Guid(id)).FirstOrDefault();
                    if (v != null)
                    {
                        v.AdminType = true;
                        var changepass = v.Password;
                        v.Password = rnd.Next(0003000, 99999999).ToString();
                        v.GlobalID = Guid.NewGuid();
                        dc.SaveChanges();
                        SendChangePassword(v.Email, v.GlobalID.ToString(), v.Password);
                        Status = true;
                        message = "Succefully Change Your Password";
                    }
                    else
                    {
                        message = "Invalid Request";
                        Status = false;
                    }

                }
            }
            catch (Exception)
            {

                message = "Invalid Request";
                Status = false;
            }
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return RedirectToAction("Login", "Login");
        }

        Random rnd = new Random();
        [HttpPost]
        public async Task<ActionResult> Forget(LoginCred user, string emailID, string activationcode, string oldpass)
        {
            bool Status = false;
            string message = "";
            if (!ModelState.IsValid)
            {

                emailID = user.Email;

                Random rnd = new Random();
                #region //Email is already Exist Check
                var isExist = IsEmailExist(user.Email);
                if (isExist)
                {
                    using (InventoryDBEntities dc = new InventoryDBEntities())
                    {
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        var EC = dc.UserLogins.Where(a => a.Email == emailID).FirstOrDefault();

                        if (EC != null)
                        {
                            try
                            {
                                EC.GlobalID = Guid.NewGuid();
                                dc.SaveChanges();
                                ForgetPassChange(EC.GlobalID.ToString());
                                message = "Forget Password Link has been successfully Sent To your Email Account Please Check Your Email Account:  " + user.Email;
                                Status = true;
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else
                        {
                            message = "Invalid Request";
                            Status = false;
                        }
                        //Send Email to Users
                        return RedirectToAction("Login", "Login");

                    }
                }

            }
            return View();
        }

        [HttpGet]
        public ActionResult ChangePassword(string id)
        {
            ViewBag.Status = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(AdvMasterDetails.Models.LoginCred user, string emailID, string activationcode, string oldpass, string id)
        {
            bool status = false;
            string message = "";
            using (InventoryDBEntities dc = new InventoryDBEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;// Avoid Confirmation password does not match on save changes
                try
                {
                    var v = dc.UserLogins.Where(a => a.GlobalID == new Guid(id)).FirstOrDefault();
                    if (v != null)
                    {
                        var changePass = user.Password;
                        changePass = System.Web.Helpers.Crypto.Hash(user.Password);
                        v.GlobalID = Guid.NewGuid();
                        v.Password = changePass;
                        dc.SaveChanges();
                        status = true;
                        message = "Your Account Password is changed and your password has been sent to your registered Email address.Please Check your email id for updated new password .";
                        SendChangePassword(v.Email, "", user.Password.ToString());
                    }
                    else
                    {
                        message = "Invalid Request";
                        status = false;
                    }

                }
                catch (Exception)
                {

                    message = "Invalid Request";
                    status = false;

                }
            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();

        }
        string sendText;
        [NonAction]
        public void  SendChangePassword(string emailID, string activationcode, string userpass)
        {
            var verifyUrl = "/PassReset/ChangePassword/" + activationcode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("saurabh@promaxevents.in", "🔴 Promax Password");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Kashyap007";
            string subject = "🔴 Promax Account Password 🔴";

            if ((userpass != "") && (userpass.Length >= 8))
            {
                sendText = "<br/><br/>Your Promax Account Password is : <b style='font-size:16px;color:#212121'>" + userpass + "</b><br/><br/>Your Click Here For Set Own New Password : " + link;
            }
            else
            {
                sendText = "<br/><br/>Your New Password is : " + userpass;
            }
            string body = sendText.ToString();
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);

        }
    }

}
#endregion