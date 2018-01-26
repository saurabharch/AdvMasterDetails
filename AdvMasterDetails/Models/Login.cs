using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is Required")]
        public string Email { get; set; }

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
    }

}