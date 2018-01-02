using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace AdvMasterDetails
{
 

    public class OrderListItems
    {
        //public List<string> OrderItesm { get; set; }
        public Guid gid { get; set; }
        public int OrderID { get; set; }
        public string prodName { get; set; }
        public int prodID { get; set; }
        public int prodQuantity { get; set; }
        public string prodState { get; set; }
        public string prodRemarks { get; set; }
    }
}
