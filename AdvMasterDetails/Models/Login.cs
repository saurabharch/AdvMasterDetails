using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvMasterDetails.Models
{

    [MetadataType(typeof(LoginCred))]
    public partial class Login
    {
        public string ConfirmPassword { get; set; }
    }
    public class LoginCred
    {
        [Display(Name = "Email ID")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter Valid Email ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is Required")]
        public string Email { get; set; }

        [StringLength(30, MinimumLength = 2, ErrorMessage = "Username Must be Minimum 2 Charaters")]
        [System.Web.Mvc.Remote("IsUserExists", "Login", ErrorMessage = "Is Exist")]
        public string UserName { get; set; }

        [Display(Name ="Password")]
        [MaxLength(30, ErrorMessage = "Password cannot be Greater than 30 Charaters")]
        [StringLength(31, MinimumLength = 7, ErrorMessage = "Password Must be Minimum 7 Charaters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm Password and Password Do Not Match")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        [Display(Name = "Check For Admin Login")]
        public bool AdminType { get; set; }
        public bool SuperAdmin { get; set; }
        public string ProfilePicPath { get; set; }
        public byte[] imgsrc { get; set; }

        [Display(Name = "Role")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Select Role")]
        public string Role { get; set; }
        [Display(Name = "Nickname")]
        [DataType(DataType.PhoneNumber)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nickname Required")]
        public string contact { get; set; }
        public string State { get; set; }
        public string NotiyMsg { get; set; }
        public string JoiningDate { get; set; }
        public string LastLoginTime { get; set; }
        public bool AccountActive { get; set; }
        [Display(Name = "OTP")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "OTP Required")]
        public decimal OtpP { get; set; }
    }

}