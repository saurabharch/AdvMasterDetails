//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AdvMasterDetails
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserLogin
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public System.Guid SecretKey { get; set; }
        public Nullable<bool> AdminType { get; set; }
    }
}