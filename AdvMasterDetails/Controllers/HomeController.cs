using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using System.Text;
using AdvMasterDetails;
using System.Globalization;
using System.Web.Security;
using System.Security.Principal;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace AdvMasterDetails.Controllers
{
    public class HomeController : Controller
    {
        InventoryDBEntities1 dc = new InventoryDBEntities1();
        public ActionResult Index()
        {
            return View();
        }

        //Get Categorie List
        [HttpGet]
        public JsonResult getProductCategories()
        {
            List<Category> categories = new List<Category>();
            using (InventoryDBEntities1 dc = new InventoryDBEntities1())
            {
                categories = dc.Categories.OrderBy(a => a.CategortyName).ToList();
            }
            return new JsonResult { Data = categories, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Get Product List
        [HttpGet]
        public JsonResult getProducts(int categoryID)
        {
            List<Product> products = new List<Product>();
            using (InventoryDBEntities1 dc = new InventoryDBEntities1())
            {
                products = dc.Products.Where(a => a.CategoryID.Equals(categoryID)).OrderBy(a => a.ProductName).ToList();
            }
            return new JsonResult { Data = products, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        //[HttpGet]
        //public JsonResult GetRemarks()
        //{
        //    List<RemarksTable> Remarks = new List<RemarksTable>();
        //    using (InventoryDBEntities1 dc = new InventoryDBEntities1())
        //    {
        //        Remarks = dc.RemarksTables.OrderBy(a => a.Remark).ToList();
        //    }
        //    return new JsonResult { Data = Remarks, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
       
        [HttpGet]
        public JsonResult ApprovedOrderList(bool check)
        {
            List<OrderMaster> Approve = new List<OrderMaster>();
            try
            {
                using (InventoryDBEntities1 dc = new InventoryDBEntities1())
                {
                    dc.Configuration.LazyLoadingEnabled = false;
                    Approve = dc.OrderMasters.Where(a => a.AdminApproved==check).OrderByDescending(a => a.OrderDate).ToList();
                    //Approve = dc.OrderMasters.Where(a => a.AdminApproved == check).ToList();
                }
                 return new JsonResult { Data = Approve.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = Approve.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public ActionResult AprrovedOrder()
        {
            return View();
        }
        public JsonResult OrderList(string OrderNumber)//Find Order List By Order ID//
        {
            var result = "";
            if (IsOrderNumberExist(OrderNumber))
            {
                InventoryDBEntities1 _objError = new InventoryDBEntities1();

                try
                {
                    var ErrorResult = _objError.OrderMasters.Where(a => a.OrderNo == OrderNumber).FirstOrDefault();
                    int order = ErrorResult.OrderID;
                    var OrdersList = _objError.OrderDetails.Where(a => a.OrderID == order).ToList();
                    result = JsonConvert.SerializeObject(OrdersList, Formatting.None,
                               new JsonSerializerSettings
                               {
                                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                               });
                }
                catch (Exception ex)
                {

                    result = "Invalid Request Parameter";
                }

            }
            else
            {
                result = "This is an Invalid Order Id Request";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        //Check Weather Order Id is Exist or not
        public bool IsOrderNumberExist(string Number)
        {
            using (InventoryDBEntities1 de = new InventoryDBEntities1())
            {
                var EC = de.OrderMasters.Where(a => a.OrderNo == Number).FirstOrDefault();
                return EC != null;// if not equal to null means True
            }
        }
        //Find Order List By Order ID
        //[HttpGet]
        //public JsonResult OrderList(string OrderNumber)
        //{
        //  // List<OrderDetail> OrderL = new List<OrderDetail>();
        //    JavaScriptSerializer j = new JavaScriptSerializer();
        //    InventoryDBEntities dc = new InventoryDBEntities();

        //        dc.Configuration.ProxyCreationEnabled = false;
        //        var orderId = dc.OrderMasters.Where(a => a.OrderNo == OrderNumber).FirstOrDefault();
        //        int order = orderId.OrderID;
        //        //var OrdersList = dc.OrderDetails.Where(a => a.OrderID == order).ToList();
        //      var  OrderL = dc.OrderDetails.Where(a => a.OrderID == order).ToList();
        //    //return Json(dc.OrderDetails.Where(a => a.OrderID == order).ToList(), JsonRequestBehavior.AllowGet);
        //    // return new JsonResult { Data = OrderL, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //    return j.Serialize(OrderL);
        //}
        [Authorize]
        [HttpGet]        //Get Remarks List
        public JsonResult Remarks()
        {
            List<Remark> Remark = new List<Remark>();
            using (InventoryDBEntities1 dc = new InventoryDBEntities1())
            {
                Remark = dc.Remarks.OrderBy(a => a.RemarkId).ToList();
            }
            return new JsonResult { Data = Remark, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        public JsonResult RemarkProductList(string Orderid)
        {
            List<Product> Remark = new List<Product>();
            var Hello ="";
            
            using (InventoryDBEntities1 dc = new InventoryDBEntities1())
            {
                try
                {
                    var Id = dc.OrderMasters.Where(a => a.OrderNo == Orderid).FirstOrDefault();
                    int OrderNumber = Id.OrderID;
                    var OrderList = dc.OrderDetails.Where(a => a.OrderID == OrderNumber).ToList();
                    for (int i = 0; i <= OrderList.Count - 1; i++)
                    {
                        int Producid = OrderList[i].ProductID;
                        Guid RemarkId = OrderList[i].GUID;
                        int orderid = OrderList[i].OrderID;
                        var RemarkGui = dc.RemarksTables.Where(p => p.GUID == RemarkId).FirstOrDefault();
                        var Name = dc.Products.Where(a => a.ProductID == Producid).FirstOrDefault();
                        Hello += "ProductName:" + Name.ProductName + ",Id:" + RemarkGui.GUID + ",Remark:" + RemarkGui.Remark + ",Status:" + RemarkGui.Status + ",";


                    }
                }
                catch (Exception ex)
                {

                    Hello = "Invalid Request/ Invalid Order Number";
                }
                //Hello = JsonConvert.SerializeObject(Hello, Formatting.None,
                //               new JsonSerializerSettings
                //               {
                //                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //               });

            }
            return new JsonResult { Data = Hello, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //Get Status
        [HttpGet]
        public JsonResult GetStatus()
        {
            List<Status> State = new List<Status>();
            using (InventoryDBEntities1 dc = new InventoryDBEntities1())
            {
                State = dc.Status.OrderBy(a => a.OptionStatus).ToList();
            }
            return new JsonResult { Data = State, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Save & Approval , WareHouse , Customer Copy Mail
        [HttpPost]
        public JsonResult save(OrderMaster order, RemarksTable RT,OrderDetail OD)
        {
            bool status = false;
            string OrderList = "";
            string OrderListC = "";
            int TotalQuantitySum = 0;
            string OrderNo = "";
            string CompanyName = "";
            string orderDate = "";
            try
            {
                DateTime dateOrg =DateTime.UtcNow;
                string date = Convert.ToString(dateOrg.Date + "/" + dateOrg.Month + "/" + dateOrg.Year + " " + dateOrg.Hour + ":" + dateOrg.Minute + ":" + dateOrg.Second);
                var isValidDate = DateTime.TryParseExact(order.OrderDateString, "mm-dd-yyyy", null, System.Globalization.DateTimeStyles.None, out dateOrg);
                if (isValidDate)
                {
                    //order.OrderDate = date;
                   orderDate = order.OrderDateString;
                    //order.OrderDate = orderDate.To
                }
                var isValidModel = TryUpdateModel(order);
                if (isValidModel)
                {
                    using (InventoryDBEntities1 dc = new InventoryDBEntities1())
                    {
                        order.FromDate = order.FromDate;
                        order.ToDate = order.ToDate;
                        order.Company = order.Company;
                        order.OrderDate = order.OrderDateString;
                        if (order.Status != null)
                        {

                        }
                        else
                        {
                            order.Status = "Un-Approved";

                        }
                        if (order.Remarks != null)
                        {

                        }
                        else
                        {
                            order.Remarks = "Not Return";
                        }
                        if (order.AdminApproved == false)
                        {

                        }
                        else
                        {
                            order.AdminApproved = false;
                        }
                        order.GUID = Guid.NewGuid();
                        dc.OrderMasters.Add(order);
                        dc.SaveChanges();

                        // SendEmailForApproved(order.Email, msg, order.GUID.ToString());
                        status = true;
                        if (status == true)
                        {
                            dc.Configuration.ValidateOnSaveEnabled = false;
                            string Gid = Convert.ToString(order.GUID);
                            var Guid = dc.OrderMasters.Where(x => x.GUID == new Guid(Gid)).FirstOrDefault();
                            int OrderId = Guid.OrderID;
                            OrderNo = Guid.OrderNo;
                            CompanyName = Guid.Company;
                            var OrderGlobalID = dc.OrderDetails.Where(p => p.OrderID == OrderId).ToList();
                            for (int i = 0; i <= OrderGlobalID.Count-1; i++)
                            {
                                RT.GUID = OrderGlobalID[i].GUID;
                                RT.OrderDetailsID = OrderGlobalID[i].OrderDetailsID;
                                RT.Remark = "Under Review";
                                RT.Status = "Waiting For Admin Review";
                                RT.UpdateTime = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString();
                                dc.RemarksTables.Add(RT);
                                dc.SaveChanges();
                                //OrderGlobalID[i].FromDate = order.FromDate;
                                //OrderGlobalID[i].ToDate = order.ToDate;
                                //dc.OrderDetails.Add(OrderGlobalID[i]);
                                //dc.SaveChanges();
                                Guid ArrayGuid = OrderGlobalID[i].GUID;
                                var ProductIDD = dc.OrderDetails.Where(x => x.GUID == ArrayGuid).FirstOrDefault();
                                int ProductId = ProductIDD.ProductID;
                                int Quantity = ProductIDD.Quantity;
                                var ProName = dc.Products.Where(p => p.ProductID == ProductId).FirstOrDefault();
                                string ProductName = ProName.ProductName.ToString();
                                string ProductQuantity = Convert.ToString(Quantity);
                                int ProductMax = Convert.ToInt16(ProName.CountMax);
                                int ProducntMin = Convert.ToInt16(ProName.CountMin);
                                int SrNo = i + 1;
                                int CountShort = ProductMax - Quantity;
                                if (CountShort >= 0)
                                {
                                    if (SrNo % 2 == 0)
                                    {
                                        OrderList += "<tr><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' > " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. ☑️</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + CountShort + "</td></tr> ";
                                        OrderListC += "<tr><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' > " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. ☑️</td></tr>";
                                    }
                                    else
                                    {
                                        OrderList += "<tr bgcolor='#DADADA'><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. ☑️</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + CountShort + "</td></tr> ";
                                        OrderListC += "<tr bgcolor='#DADADA'><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. ☑️</td></tr> ";
                                    }

                                }
                                else if (CountShort < 0)
                                {
                                    if (SrNo % 2 == 0)
                                    {
                                        OrderList += "<tr><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #d4d4d4;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. 🔴</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + CountShort + "</td></tr> ";
                                        OrderListC += "<tr><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #d4d4d4;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. ☑️</td></tr>";
                                    }
                                    else
                                    {
                                        OrderList += "<tr bgcolor='#DADADA'><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' > " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. 🔴</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + CountShort + "</td></tr> ";
                                        OrderListC += "<tr bgcolor='#DADADA'><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' > " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. ☑️</td></tr> ";
                                    }
                                    // OrderList += "<tr><td>" + SrNo + ".</td><td > " + ProductName + " </td><td>" + ProductQuantity + " Qty. ☑️</td><td>" + CountShort + "</td></tr> ";
                                }
                                TotalQuantitySum = Quantity + TotalQuantitySum;
                               
                            }
                            
                            var verifyUrl = "/Home/ApprovedOrder/" + Guid.GUID;
                            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                            string ReplaceTitleBody = "</tbody>" +
            "</table>" +
            "</td>" +
            "</tr>" +
            "<tr>" +
            "<td>" +
            "<table align='center' cellpadding='0' cellspacing='0' border='0'>" +
            "<tbody>" +
            "<tr>" +
            "<td style='margin-top:2px;padding:1px 2px'>" +
"<tbody>" +
"<tr>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Sr No.</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:400px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Item Name</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Quantity</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Short</th>" +
"</tr>" +
"</tbody>" +
"<tbody style='border:1px solid rgba(0,0,0,0.1);text-align:center;font-size:12px;font-weight:600'>" +
"" + OrderList + "" +
 "</tbody>" +
 "</table>" +
 "</td></tr>" +
 "<tr><td>" +
 "<table colspan='2' align='center' cellpadding='0' cellspacing='1' border='0'>" +
 "<tbody>" +
 "<tr>" +
 "<th style='background-color:rgb(175,170,170);color:rgb(36,33,33);font-size:16px;width:449px;height:25px;padding:6px;'>Total</th>" +
 "<th style='background-color:#008a00;color:rgb(255,255,255);font-size:16px;width:273px;height:25px;padding:6px;'>" + TotalQuantitySum + "</th>" +
 "</tr>" +
 "</tbody>" +
 "</table>" +
 "</td>" +
 "</tr>" +
 "<tr><td>";
                            string MainBody = "<table align = 'center' cellpadding = '0' cellspacing = '0' border = '0' > " +
            "<tbody><tr><td style='font-size:30px;font-weight:600;'><h3>🚚 Order List</h3>" +
            "</td></tr>" +
            "</tbody>" +
            "</table>" +
            "</td>" +
            "</tr>" +
            "<tr>" +
            "<td>" +
            "<table align='center' cellpadding='0' cellspacing='0' border='0'>" +
            "<tbody>" +
            "<tr>" +
            "<td style='margin-top:2px;padding:1px 2px'>" +
"<tbody>" +
"<tr>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Sr No.</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:400px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Item Name</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Quantity</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Short</th>" +
"</tr>" +
"</tbody>" +
"<tbody style='border:1px solid rgba(0,0,0,0.1);text-align:center;font-size:12px;font-weight:600'>" +
"" + OrderList + "" +
 "</tbody>" +
 "</table>" +
 "</td></tr>" +
 "<tr><td>" +
 "<table colspan='2' align='center' cellpadding='0' cellspacing='1' border='0'>" +
 "<tbody>" +
 "<tr>" +
 "<th style='background-color:rgb(175,170,170);color:rgb(36,33,33);font-size:16px;width:449px;height:25px;padding:6px;'>Total</th>" +
 "<th style='background-color:#008a00;color:rgb(255,255,255);font-size:16px;width:273px;height:25px;padding:6px;'>" + TotalQuantitySum + "</th>" +
 "</tr>" +
 "</tbody>" +
 "</table>" +
 "</td>" +
 "</tr>" +
 "<tr><td>";
                            string MainBodyW = "<tbody>" +
            "<tr>" +
            "<td style='margin-top:2px;padding:1px 2px'>" +
"<tbody>" +
"<tr>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Sr No.</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:400px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Item Name</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Quantity</th>" +
"</tr>" +
"</tbody>" +
"<tbody style='border:1px solid rgba(0,0,0,0.1);text-align:center;font-size:12px;font-weight:600'>" +
"" + OrderListC + "" +
 "</tbody>" +
 "</table>" +
 "</td></tr>" +
 "<tr><td>" +
 "<table colspan='2' align='center' cellpadding='0' cellspacing='1' border='0'>" +
 "<tbody>" +
 "<tr>" +
 "<th style='background-color:rgb(175,170,170);color:rgb(36,33,33);font-size:16px;width:578px;height:25px;padding:6px;'>Total</th>" +
 "<th style='background-color:#008a00;color:rgb(255,255,255);font-size:16px;width:162px;height:25px;padding:6px;'>" + TotalQuantitySum + "</th>" +
 "</tr>" +
 "</tbody>" +
 "</table>" +
 "</td>" +
 "</tr>" +
 "<tr>" + "<td>" + "";
                            string ButtonLink = "<table style='border:0px solid rgba(0,0,0,0.1);color:#153643;font-family:sans-serif;font-size:16px;line-height:22px;padding:20px 6px'>" +
                             "<tbody>" +
                             "<tr><td>" +
                             "<a style= 'background-color:#34a853;border-bottom-left-radius:3px;border-bottom-right-radius:3px;border-color:#34a853;border-radius:3px;border-style:solid;border-top-left-radius:3px;border-top-right-radius:3px;border-width:10px 12px 10px 12px;color:#ffffff;display:inline-block;font-family:Roboto,Arial;font-size:14px;font-weight:500;line-height:18px;margin:0;text-align:center;text-decoration:none' href = '"+link+"' bgcolor='#34A853' align='center' target='_blank' >"+
        "<span align='center' style='display:block;padding-left:6px;padding-right:6px'>Approved</span></a>"+
                                         //"<a href='" + link + "' rel='noopener noreferrer' style='background-color:#1fe639bb;border-radius:4px;border:1px solid #1fe639bb;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:15px;line-height:40px;margin:0;padding:0;text-align:center;text-decoration:none;width:150px;font-weight:600' target='_blank' data-saferedirecturl='https://www.google.com/url?hl=en&amp;q=http://www.google.com&amp;source=gmail&amp;ust=1508911257103000&amp;usg=AFQjCNH48CGQlRWF0EpFA_gLPn7CLHHXkw'>Approved</a>" +
                                         "</td>" +
                             //"<td>"+
                             //"<a href='http://www.google.com' rel='noopener noreferrer' style='background:#f03333;border-radius:4px;border:1px solid #c23f3f;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:15px;line-height:40px;margin:0;padding:0;text-align:center;text-decoration:none;width:150px;font-weight:600' target='_blank' data-saferedirecturl='https://www.google.com/url?hl=en&amp;q=http://www.google.com&amp;source=gmail&amp;ust=1508911257103000&amp;usg=AFQjCNH48CGQlRWF0EpFA_gLPn7CLHHXkw'>Closed</a>"+
                             //"</td>"+
                             "</tr>" +
                             "</tbody>" +
                             "</table>" + "";
                            string MainBody1 = "<table colspan='3' align='center' cellpadding='0' cellspacing='0' border='0' style='border:0px solid rgba(0,0,0,0.1);color:#153643;font-size:16px;line-height:22px;padding:4px 6px;background-color:#c4cccc69;border-radius:2%;'>" +
                           "<tbody style='border:1px solid rgba(0,0,0,0.1);text-align:center;font-size:12px;font-weight:600;' >" +
                            "<tr>" +
                            "<td class='display'>" +
                            "<a href='#' style='display:inline-block; padding: 5px 10px; color:#fff;background-color:#155a93;text-decoration:none;border-radius:3px'>" +
                           "YOUR ORDER HAS BEEN PLACED PLEASE WAIT FOR ADMIN REVIEW" +
                           "</a>" +
                           "</td>" +
                           "</tr>" +
                           "</tbody>" +
                           "</table>";
                            string SubAdmin = "🚚 New Order List";
                            string SubCus = "🚚 Your Order List";
                            string SubCuss = "🚚 Your Order List Status";
                            SendEmailForApproved(order.Email, SubAdmin, CompanyName, OrderNo, MainBody.ToString() + ButtonLink.ToString(), OrderList, TotalQuantitySum, order.ContactNo, order.FromDate, order.ToDate, order.GUID.ToString(), orderDate.ToString());
                            SendEmailToWareHouse(order.Email, SubAdmin, CompanyName, OrderNo, ReplaceTitleBody.ToString(), OrderListC, TotalQuantitySum, order.ContactNo, order.FromDate, order.ToDate, order.GUID.ToString(), orderDate.ToString());
                            CustomerCopyEmail(order.Email, SubCus, CompanyName, OrderNo, "", MainBodyW.ToString(), OrderListC, TotalQuantitySum, order.ContactNo, order.FromDate, order.ToDate, order.GUID.ToString(), orderDate.ToString());
                            CustomerCopyEmail(order.Email, SubCuss, CompanyName, OrderNo, "", MainBody1.ToString(), OrderListC, TotalQuantitySum, order.ContactNo, order.FromDate, order.ToDate, order.GUID.ToString(), orderDate.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new JsonResult { Data = new { status = status } };
        }

        // Approved Confirmation Mail
        [HttpGet]
        public ActionResult ApprovedOrder(string id, RemarksTable RT)
        {
            bool Status = false;
            string message = "";
            string OrderNo = "";
            string CompanyName = "";
            string OrderList = "";
            int TotalQuantitySum = 0;
            int CurrentProduct = 0;
            string orderDate = "";
            try
            {
                using (InventoryDBEntities1 dc = new InventoryDBEntities1())
                {
                    dc.Configuration.ValidateOnSaveEnabled = false; // Avoid Confirmation password does not match on save changes
                    var v = dc.OrderMasters.Where(a => a.GUID == new Guid(id)).FirstOrDefault();
                    if ((v != null) && (v.AdminApproved == false))
                    {
                        v.AdminApproved = true;
                        //v.ActivationCode = Guid.NewGuid();
                        v.Remarks = "Approved";
                        v.Status = "Approved";
                        dc.SaveChanges();
                        Status = true;
                        string ComName = v.Company;
                        message = ComName + " Order Has Been Approved";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        // string Gid = Convert.ToString(id);
                        //// var Guid = dc.OrderMasters.Where(x => x.GUID == new Guid(Gid)).FirstOrDefault();
                        int OrderId = v.OrderID;
                        OrderNo = v.OrderNo;
                        orderDate = v.OrderDate;
                        CompanyName = v.Company;
                        var OrderGlobalID = dc.OrderDetails.Where(p => p.OrderID == OrderId).ToList();

                        for (int i = 0; i <= OrderGlobalID.Count - 1; i++)
                        {
                            RT.GUID = OrderGlobalID[i].GUID;
                            OrderGlobalID[i].Status = "Approved - Package";
                            OrderGlobalID[i].Remarks = "Warehouse - package";
                            dc.OrderDetails.Add(OrderGlobalID[i]);
                            dc.SaveChanges();
                            var RemarkAdd = dc.RemarksTables.Where(a => a.GUID == RT.GUID).FirstOrDefault();
                            RemarkAdd.Remark = "Approved For Packaging";
                            RemarkAdd.Status = "Warehouse - packaging";
                            RemarkAdd.UpdateTime = DateTime.Now.ToShortDateString()+" - "+DateTime.Now.ToLongTimeString();
                            //dc.RemarksTables.Add(RemarkAdd);
                            dc.SaveChanges();
                            string RTGUID = Convert.ToString(RT.GUID);
                            var ProductIDD = dc.OrderDetails.Where(x => x.GUID == RT.GUID).FirstOrDefault();
                            int ProductId = ProductIDD.ProductID;
                            int Quantity = ProductIDD.Quantity;
                            var ProName = dc.Products.Where(p => p.ProductID == ProductId).FirstOrDefault();
                            string ProductName = ProName.ProductName.ToString();
                            string ProductQuantity = Convert.ToString(Quantity);
                            int CountProduct = Convert.ToInt16(ProName.CountMax - Quantity);
                            int ProductAvailable = Convert.ToInt16(Quantity + CountProduct);

                            int SrNo = i + 1;
                            if (CountProduct > 0)
                            {
                                if (SrNo % 2 == 0)
                                {
                                    OrderList += "<tr><td  border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. ☑️</td></tr> ";
                                }
                                else
                                {
                                    OrderList += "<tr bgcolor='DADADA'><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. ☑️</td></tr> ";
                                }
                                //  OrderList += "<tr><td>" + SrNo + ".</td><td > " + ProductName + " </td><td>" + CountProduct + " Qty. ☑️</td></tr> ";
                                CurrentProduct = Quantity;
                            }
                            else if (ProductAvailable > 0)
                            {
                                if (SrNo % 2 == 0)
                                {
                                    OrderList += "<tr><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. ☑️</td></tr> ";
                                }
                                else
                                {
                                    OrderList += "<tr bgcolor='DADADA'><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + Quantity + " Qty. ☑️</td></tr> ";
                                }


                                CurrentProduct = Quantity;
                            }
                            //else if(CountProduct<=0)
                            //{
                            //    OrderList += "<tr><td>" + SrNo + ".</td><td > " + ProductName + " </td><td>" + CountProduct + " Qty. 🚫</td></tr> ";
                            //}

                            // OrderList += "<tr><td>" + SrNo + ".</td><td > " + ProductName + " </td><td>" + ProductQuantity + " Qty.</td></tr> ";
                            TotalQuantitySum = CurrentProduct + TotalQuantitySum;

                        }
                        string MainBody = "<table><tr><td style='font-size:16;font-weight:600;width:auto;'><a href='#' style='display:inline-block; padding: 5px 10px; color:#fff;background-color:#155a93;text-decoration:none;border-radius:3px;font-size:18px;'>Approved<a/></td></tr></table>" +
                            "<tbody>" +
"<tr>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Sr No.</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:400px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Item Name</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Quantity</th>" +
"</tr>" +
"</tbody>" +
"<tbody style='border:1px solid rgba(0,0,0,0.1);text-align:center;font-size:12px;font-weight:600'>" +
"" + OrderList + "" +
 "</tbody>" +
 "</table>" +
 "</td></tr>" +
 "<tr><td>" +
 "<table colspan='2' align='center' cellpadding='0' cellspacing='1' border='0'>" +
 "<tbody>" +
 "<tr>" +
 "<th style='background-color:rgb(175,170,170);color:rgb(36,33,33);font-size:16px;width:571px;height:25px;'>Total</th>" +
 "<th style='background-color:#008a00;color:rgb(255,255,255);font-size:16px;width:166px;height:25px;padding:8px 0px;'>" + TotalQuantitySum + "</th>" +
 "</tr>" +
 "</tbody>" +
 "</table>" + "";
                        string MainBody1 = "<tbody>" +
                       "<tr>" +
                       "<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Sr No.</th>" +
                       "<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:400px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Item Name</th>" +
                       "<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Quantity</th>" +
                       "</tr>" +
                       "</tbody>" +
                       "<tbody style='border:1px solid rgba(0,0,0,0.1);text-align:center;font-size:12px;font-weight:600'>" +
                       "" + OrderList + "" +
                        "</tbody>" +
                        "</table>" +
                        "</td></tr>" +
                        "<tr><td>" +
                        "<table colspan='2' align='center' cellpadding='0' cellspacing='1' border='0'>" +
                        "<tbody>" +
                        "<tr>" +
                        "<th style='background-color:rgb(175,170,170);color:rgb(36,33,33);font-size:16px;width:582px;height:25px;'>Total</th>" +
                        "<th style='background-color:#008a00;color:rgb(255,255,255);font-size:16px;width:166px;height:25px;padding:8px 0px;'>" + TotalQuantitySum + "</th>" +
                        "</tr>" +
                        "</tbody>" +
                        "</table>" + "";
                        string PraMsg = "<tr><td style='font-size:24px;padding-left:10px;height:50px;color:#1ac71aa8;'><b>👍 CONFIRMED<b></td></tr><tr><td style='font-size:14px;padding-left:10px;height:50px;color:black;'>Your Package is Ready for Packaging! For More Contact on These Numbers. 📞 +91 9623 718383 , 8378 972253<br/></td></tr>";
                        string WareHouseSub = "🚚 New Approved Order List";
                        string CusSub = "🚚 Your Approved Order List";
                        SendEmailToWareHouse(v.Email, WareHouseSub, CompanyName, OrderNo, MainBody, OrderList, TotalQuantitySum, v.ContactNo, v.FromDate, v.ToDate, v.GUID.ToString(), orderDate.ToString());
                        CustomerCopyEmail(v.Email, CusSub, v.Company, OrderNo, PraMsg, MainBody1, OrderList, TotalQuantitySum, v.ContactNo, v.FromDate, v.ToDate, v.GUID.ToString(), v.OrderDate);
                        Status = true;
                    }
                    else
                    {
                        message = "Order is Already Approved / Invalid Request";
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
            return View();
        }

        //Ready to cart Mail
        [Authorize]
        [HttpGet]
        public ActionResult OrderStatus(string id, RemarksTable RT)
        {
            bool Status = false;
            string message = "";
            string OrderNo = "";
            string CompanyName = "";
            string OrderList = "";
            int TotalQuantitySum = 0;
            id = id.Replace(" ", "");
            // string Orderstate = "";
            try
            {
                using (InventoryDBEntities1 dc = new InventoryDBEntities1())
                {
                    dc.Configuration.ValidateOnSaveEnabled = false; // Avoid Confirmation password does not match on save changes
                    var v = dc.OrderMasters.Where(a => a.GUID == new Guid(id)).FirstOrDefault();
                    if ((v != null) && (v.AdminApproved == true))
                    {
                        Status = true;
                        v.Status = "In Cart";
                        v.Remarks = "Ready For Cart";
                        dc.SaveChanges();
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        string Gid = Convert.ToString(id);
                        var Guid = dc.OrderMasters.Where(x => x.GUID == new Guid(Gid)).FirstOrDefault();
                        int OrderId = Guid.OrderID;
                        OrderNo = Guid.OrderNo;
                        CompanyName = Guid.Company;
                        message = CompanyName + " Order Has Been Approved";
                        var OrderGlobalID = dc.OrderDetails.Where(p => p.OrderID == OrderId).ToList();
                        for (int i = 0; i <= OrderGlobalID.Count - 1; i++)
                        {
                            RT.GUID = OrderGlobalID[i].GUID;
                            OrderGlobalID[i].Status = "Ready - In Cart";
                            OrderGlobalID[i].Remarks = "Ready - In Cart";
                           // dc.OrderDetails.Add(OrderGlobalID[i]);
                            dc.SaveChanges();
                            string RTGUID = Convert.ToString(RT.GUID);
                            var ProductIDD = dc.OrderDetails.Where(x => x.GUID == RT.GUID).FirstOrDefault();
                            int ProductId = ProductIDD.ProductID;
                            int Quantity = 0;
                            Quantity = ProductIDD.Quantity;
                            var ProName = dc.Products.Where(p => p.ProductID == ProductId).FirstOrDefault();
                            string ProductName = ProName.ProductName.ToString();
                            string ProductQuantity = Convert.ToString(Quantity);
                            int ProductMax = Convert.ToInt16(ProName.CountMax);
                            //int ProducntMin = Convert.ToInt16(ProName.CountMin);
                            int SrNo = i + 1;
                            //int CountShort = ProductMax - Convert.ToInt16(ProductQuantity);
                            if (SrNo % 2 == 0)
                            {
                                OrderList += "<tr><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + ProductQuantity + " Qty.</td></tr> ";
                            }
                            else
                            {
                                OrderList += "<tr bgcolor='DADADA'><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + ProductQuantity + " Qty.</td></tr> ";
                            }
                            TotalQuantitySum = Quantity + TotalQuantitySum;

                        }
                        string MainBody = "<tbody>" +
"<tr>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Sr No.</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:400px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Item Name</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Quantity</th>" +
"</tr>" +
"</tbody>" +
"<tbody style='border:1px solid rgba(0,0,0,0.1);text-align:center;font-size:12px;font-weight:600'>" +
"" + OrderList + "" +
"</tbody>" +
"</table>" +
"</td></tr>" +
"<tr><td>" +
"<table colspan='2' align='center' cellpadding='0' cellspacing='1' border='0'>" +
"<tbody>" +
"<tr>" +
"<th style='background-color:rgb(175,170,170);color:rgb(36,33,33);font-size:16px;width:571px;height:25px;'>Total</th>" +
"<th style='background-color:#008a00;color:rgb(255,255,255);font-size:16px;width:166px;height:25px;padding:8px 0px;'>" + TotalQuantitySum + "</th>" +
"</tr>" +
"</tbody>" +
"</table>" + "";
                        string ParaMsg = "<tr><td style='font-size:24px;padding-left:10px;height:50px;color:#1ac71aa8;'><b>Your Package is Ready!<b></td></tr><tr><td style='font-size:14px;padding-left:10px;height:50px;color:black;'>Ready For 🚚 Cart! Please Contact on These Numbers. 📞 +91 9623 718383 , 8378 972253<br/>For Collecting your Order</td></tr>";
                        string CusSubj = "🚚 Your Order is Ready For Cart!";
                        CustomerCopyEmail(v.Email, CusSubj, v.Company, OrderNo, ParaMsg, MainBody, OrderList, TotalQuantitySum, v.ContactNo, v.FromDate, v.ToDate, v.GUID.ToString(), v.OrderDate);
                        Status = true;
                    }
                    else
                    {
                        message = "Order is Already Approved / Invalid Request";
                        Status = false;
                    }

                }
            }
            catch (Exception ex)
            {
                message = "Invalid Request";
                Status = false;

            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult OrderCancel(string id, RemarksTable RT)
        {
            bool Status = false;
            string message = "";
            string OrderNo = "";
            string CompanyName = "";
            string OrderList = "";
            int TotalQuantitySum = 0;
            id = id.Replace(" ", "");
            // string Orderstate = "";
            try
            {
                using (InventoryDBEntities1 dc = new InventoryDBEntities1())
                {
                    dc.Configuration.ValidateOnSaveEnabled = false; // Avoid Confirmation password does not match on save changes
                    var v = dc.OrderMasters.Where(a => a.GUID == new Guid(id)).FirstOrDefault();
                    if ((v != null) && (v.AdminApproved == true))
                    {
                        Status = true;
                        v.Status = "Order Cancel";
                        v.Remarks = "This Order is Cancel";
                        v.AdminApproved = false;
                        dc.SaveChanges();
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        string Gid = Convert.ToString(id);
                        var Guid = dc.OrderMasters.Where(x => x.GUID == new Guid(Gid)).FirstOrDefault();
                        int OrderId = Guid.OrderID;
                        OrderNo = Guid.OrderNo;
                        CompanyName = Guid.Company;
                        message = CompanyName + " Order Has Been Cancel";
                        var OrderGlobalID = dc.OrderDetails.Where(p => p.OrderID == OrderId).ToList();
                        for (int i = 0; i <= OrderGlobalID.Count - 1; i++)
                        {
                            RT.GUID = OrderGlobalID[i].GUID;
                            OrderGlobalID[i].Status = "Cancelled";
                            OrderGlobalID[i].Remarks = "Cancelled";
                           // dc.OrderDetails.Add(OrderGlobalID[i]);
                            dc.SaveChanges();
                            string RTGUID = Convert.ToString(RT.GUID);
                            var ProductIDD = dc.OrderDetails.Where(x => x.GUID == RT.GUID).FirstOrDefault();
                            int ProductId = ProductIDD.ProductID;
                            int Quantity = ProductIDD.Quantity;
                            var ProName = dc.Products.Where(p => p.ProductID == ProductId).FirstOrDefault();
                            string ProductName = ProName.ProductName.ToString();
                            string ProductQuantity = Convert.ToString(Quantity);
                            int ProductMax = Convert.ToInt16(ProName.CountMax);
                            int ProducntMin = Convert.ToInt16(ProName.CountMin);
                            int SrNo = i + 1;
                            int CountShort = ProductMax - Convert.ToInt16(ProductQuantity);
                            if (SrNo % 2 == 0)
                            {
                                OrderList += "<tr><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + ProductQuantity + " Qty.</td></tr> ";
                            }
                            else
                            {
                                OrderList += "<tr bgcolor='DADADA'><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + SrNo + ".</td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;'> " + ProductName + " </td><td border='1' style='border:1px solid #A3A3A3;padding:8px;vertical-align:top;' text-align='center'>" + ProductQuantity + " Qty.</td></tr> ";
                            }
                            TotalQuantitySum = Quantity + TotalQuantitySum;

                        }
                        string MainBody = "<tbody>" +
"<tr>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Sr No.</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:400px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Item Name</th>" +
"<th style='background-color:rgb(49,47,47);color:white;font-size:16px;width:150px;height:27px;border:1px solid #ffff;padding:8px;vertical-align:top;'>Quantity</th>" +
"</tr>" +
"</tbody>" +
"<tbody style='border:1px solid rgba(0,0,0,0.1);text-align:center;font-size:12px;font-weight:600'>" +
"" + OrderList + "" +
"</tbody>" +
"</table>" +
"</td></tr>" +
"<tr><td>" +
"<table colspan='2' align='center' cellpadding='0' cellspacing='1' border='0'>" +
"<tbody>" +
"<tr>" +
"<th style='background-color:rgb(175,170,170);color:rgb(36,33,33);font-size:16px;width:571px;height:25px;'>Total</th>" +
"<th style='background-color:#008a00;color:rgb(255,255,255);font-size:16px;width:166px;height:25px;padding:8px 0px;'>" + TotalQuantitySum + "</th>" +
"</tr>" +
"</tbody>" +
"</table>" + "";
                        string ParaMsg = "<tr><td style='font-size:24px;padding-left:10px;height:50px;color:#1ac71aa8;'><b>Your Order is Cancelled!<b></td></tr><tr><td style='font-size:14px;padding-left:10px;height:50px;color:black;'>Due to Un-Availibility of cart items! Please Contact on These Numbers. 📞 +91 9623 718383 , 8378 972253<br/>For Collecting your Order</td></tr>";
                        string CusSubj = "🚚 Your Order is Cancelled !";
                        CustomerCopyEmail(v.Email, CusSubj, v.Company, OrderNo, ParaMsg, MainBody, OrderList, TotalQuantitySum, v.ContactNo, v.FromDate, v.ToDate, v.GUID.ToString(), v.OrderDate);
                        Status = true;
                    }
                    else
                    {
                        message = "Order is Already Cancelled / Invalid Request";
                        Status = false;
                    }

                }
            }
            catch (Exception ex)
            {
                message = "Invalid Request";
                Status = false;

            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }

       
        [Authorize]
        //Load Dashboard
        [HttpGet]
        public ActionResult Dashboard()
        {
            return View();
        }

        //Product Availibility in time range
        [HttpGet]
        public ActionResult CheckAvailiblity(string fromDate, string ToDate, string ProdId,string state, string Quantity)


        {
            bool status = false;
            string data = "";
            string State = state;
            int AVProdCount=0;
            int Quant = Convert.ToInt16(Quantity);
            int ProductID = Convert.ToInt16(ProdId);
            try
            {
                //string dd = "17 November 2017 06:00 am";
                //string ff = "17 November 2017  09:00 am";
                //DateTime dateOrg;
                //DateTime dateFrom;
                //DateTime.TryParseExact(dd, "yyyy/MM/dd H:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOrg);
                //DateTime.TryParseExact(ff, "yyyy/MM/dd H:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dateFrom);
                using (InventoryDBEntities1 dc = new InventoryDBEntities1())
                {
                    var AvProdCount = dc.sp_ProductAvailibility(fromDate, ToDate, ProductID, State, Quant).FirstOrDefault();
                    AVProdCount = AvProdCount.Value;
                    if (data != null)
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;

            }
            //  return new JsonResult { Data = AVProdCount, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return View();
        }

        //Check Product Availibility in time range
        //[HttpPost]
        //public ActionResult CheckAvailiblity(string fromDate,string ToDate,string ProdId,string Quantity)
        //{
        //    bool status = false;
        //    string data="";
        //    string State = "";
        //    int Quant = Convert.ToInt16(Quantity);
        //    int ProductID = Convert.ToInt16(ProdId);
        //    try
        //    {
        //        string dd = "17 November 2017 06:00 am";
        //        string ff = "17 November 2017  09:00 am";
        //        //DateTime dateOrg;
        //        //DateTime dateFrom;
        //        //DateTime.TryParseExact(dd, "yyyy/MM/dd H:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOrg);
        //        //DateTime.TryParseExact(ff, "yyyy/MM/dd H:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dateFrom);
        //        using (InventoryDBEntities1 dc = new InventoryDBEntities1())
        //        {
        //            var AvProdCount = dc.sp_ProductAvailibility(dd, ff, ProductID, State, Quant).FirstOrDefault();
        //            if (data != null)
        //            {
        //                status = true;
        //            }
        //            else
        //            {
        //                status = false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        status = false;

        //    }
        //    return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        [NonAction]
        //Checking Product is available withing the time period as per required quantity
        public bool ProdCheckAvailiblity(string fromDate, string ToDate,int prodid,string State,int Qnt)
        {
            bool status = false;
            try
            {
                //string dd = "17 November 2017 06:00 am";
                //string ff = "17 November 2017  09:00 am";
                //DateTime dateOrg;
                //DateTime dateFrom;
                //DateTime.TryParseExact(dd, "yyyy/MM/dd H:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOrg);
                //DateTime.TryParseExact(ff, "yyyy/MM/dd H:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dateFrom);
                using (InventoryDBEntities1 dc = new InventoryDBEntities1())
                {
                    var data = dc.sp_ProductAvailibility(fromDate, ToDate, prodid, State, Qnt).FirstOrDefault();
                    if (data != null)
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;

            }
            return status;
        }
        //Send Email To Admin
        [NonAction]
        public void SendEmailForApproved(string email, string Subject, string company, string OrderNumber, string itemList, string OrderItems, int itemsCount, string Contact, string from, string To, string Guid, string OrderDate)
        {
            var verifyUrl = "/Home/ApprovedOrder/" + Guid;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("saurabh@promaxevents.in", "New Order List");
            var toEmail = new MailAddress("saurabh@promaxevents.in");
            //  var ccMail = new MailAddress("amol@promaxevents.in");
            var fromEmailPassword = "Kashyap007";
            string subject = Subject;
            string header = "<head>" +
    "<meta charset='UTF-8'>" +
    "<meta name='viewport' content='width=device-width, initial-scale=1.0'>" +
    "<meta http-equiv='X-UA-Compatible' content='ie=edge'>" +
"<style type='text/css'>" +
".body{" +
"font - family: 'Segoe UI', Tahoma, Geneva, Verdana, sans - serif;" +
       " position: absolute;" +
            "align - items: center;" +
       " }" +
        "table.container2 {" +
       " background-repeat: no-repeat;" +
        "background-position: center;" +
       " background-size: 100%;" +
   " }" +

   "table.container3 {" +
      "  background-color: rgb(49, 47, 47);" +
    "background-position: center;" +
      "background-size: 100%;" +
    "}" +
".container {" +
       "width: 100%;" +
        "background-color: White;" +
        "height: 20px;" +
        "font-weight: 200;" +
        "font-size: 12px;" +
        "font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;" +
    "}" +
".imgshift{" +
       " text-align:right;" +
        "align-items: right;" +
        "align-content: right;" +
    "}" +
"</style>" +
    "<title>Promax Cart</title>" +
"</head>" + "";

            string body = "<body>" +
            "<table align='center' cellpadding='10' cellspacing='0' style='width:729px!important;max-width:729px;border:0px solid rgba(0,0,0,0.1);background-color:#f3eeee1c;font-family:Segoe UI', Tahoma, Geneva, Verdana, sans-serif;'>" +
            "<tbody>" +
            "<tr>" +
            "<td>" + "<hr/>" +
            "<table class='container2' align='center' cellpadding='0' cellspacing='0' border='0' style='background-color:rgba(0.84,0.85,0.88,0.2);color:#153643;font-size:16px;padding:2% 2% 1%'>" +
            "<tbody>" +
            "<tr>" + "<td>" +
            "<img class='imgshift' src='http://promaxhdevents.com/wp-content/uploads/2016/07/new-logo02-e1468052055630.png'>" +
            "</td>" + "</tr>" +
            "</tbody>" +
            "</table>" +
            "<hr/>" +
            "</td></tr>" +
            "<tr><td>" +
            "<table align='center' cellpadding='0' cellspacing='0' border='0' style='padding:4px 6px;font-weight:600;color:white;margin-left:3px;font-family:Segoe UI', Tahoma, Geneva, Verdana, sans-serif;'>" +
            "<tbody>" +
            "<tr>" +
            "<td style='background-color:#302e2e8a;font-size:24px;padding-left:10px;height:50px'>" + "<b style='margin-top:8px'>Dear Promax Scientific Developers Pvt.Ltd.</b></td></tr>" +
            "<br><br>" +
            "<tr><td>" +
            "<table colspan='2' style='width:729px;margin-left:3px;margin-top:10px;background-color:#302e2e8a !important;font-family:Segoe UI', Tahoma, Geneva, Verdana, sans-serif;'>" +
            "<tbody>" +
            "<tr>" + "<td style='font-size:14px;padding-top:10px;font-weight:400;width:60%;color:white;'>🏢 Orgnisation :<b> " + company + "</b></td>" +
            "<td style='font-size:14px;font-weight:400;width:40%;padding-top:10px;color:white;'>⏰From Date :<b> " + from + "</b></td>" +
            "</tr>" +
            "<tr>" +
            "<td style='font-size:14px;font-weight:400;width:60%;padding-top:10px;color:white;'>📋 Order No.<b> # " + OrderNumber + "</b><br/><br/> 🗓️ Order Date: <b>" + OrderDate + "</b>" +
            "</td>" +
            "<td style='font-size:14px;font-weight:400;width:40%;padding-top:10px;color:white;'>⏰ To Date : <b>" + To + "</b>" +
            "</td>" +
            "</tr>" +
            "<tr>" +
            "<td style='font-size:14px;font-weight:400;width:40%;padding-top:10px;color:white;'>📞 Contact :<b> +91-" + Contact + "</b>" +
            "</td>" +
            "<td style='font-size:12px;font-weight:400;width:60%;padding-top:10px;color:white;'>✉ Email : <b>" + email + "</b>" +
            "</td>" +
            "</tr>" +
            "</tbody>" +
            "</table>" +
            "</td></tr>" +
            "</tbody>" +
            "</table>" +
            "</td></tr>" +
            "</tbody>" +
            "<table align='center' cellpadding='0' cellspacing='0' border='0'>" +
            "<tbody>" +
            "<tr>" +
            "<td style='margin-top:2px;padding:1px 2px'>" + "";
            string MainBody = itemList;
            string FooterBody = "</td>" +
             "</tr>" +
             //"<tr><td>"+
             //"<table style='width:729px' border='0' cellpadding='0' cellspacing='0'>"+
             //"<tbody>"+
             //"<tr>"+
             //"<td style='background:#d97634;color:white;font-weight:600;font-size:16px;padding:1% 1% 1%'>Promax Scientific Developers Pvt. Ltd.</td><td style='background:#d97634;padding:0% 0% -5%'>"+"<a href='https://www.facebook.com/promax.hd.7' target='_blank' data-saferedirecturl='https://www.google.com/url?hl=en&amp;q=https://www.facebook.com/promax.hd.7&amp;source=gmail&amp;ust=1508911257103000&amp;usg=AFQjCNEOC6rjpB-liAj8WGlgiAE33FZ5jw'>"+
             //"<img src='https://ci6.googleusercontent.com/proxy/pvX2YraU_BnR_JBocJKVHVaAKfyzxOe_-YUxvkHcW27nTn6pTA0SweVYCZimVRidTHQGBt_keLpt13XY72jbN-OK7zl-1IyjMOZOzKvMDSwDCygZuoDA6fnFU8Z7lCCiqRHvMzKEIHjRano-k9h4Z6Wp-eI12_wp985b_jgewQnVAoxQA9XkrnQgETLY=s0-d-e1-ft#http://images.go.mongodb.com/EloquaImages/clients/MongoDB/%7b1c5f399c-f1d3-4364-be92-c0e4536b42cf%7d_social_icon-04.png' width='40' height='40' style='display:block' class='CToWUd'></a></td><td style='background:#d97634;padding:1% 1% 1%'>"+
             //"<a href='https://twitter.com/promaxhdevents' target='_blank' data-saferedirecturl='https://www.google.com/url?hl=en&amp;q=https://twitter.com/promaxhdevents&amp;source=gmail&amp;ust=1508911257103000&amp;usg=AFQjCNGEwPDH3Dr0SIBYtuP0FOGGM5jDQQ'>"+
             //"<img src='https://ci3.googleusercontent.com/proxy/mD7m2RL1crxESqTrztL14mxMTc4ZNq-_DEEQ227dss-mvp2o8gIq0lkLhqYZdpClY7sEp-pERN0d0vZZNXh0HW_pWir0tKHn2rNxqqV89Xt_3C70tdUHGTd8IHL46mGPjaaM3FqbOg28LgqkTlpWpSTiUfMraR9iz9ZxZKJEF3AE05LDm56eNrIqVJ2d=s0-d-e1-ft#http://images.go.mongodb.com/EloquaImages/clients/MongoDB/%7b641c0845-e8fb-4991-bed7-05ccf7fe490f%7d_social_icon-01.png' width='40' height='40' style='display:block' class='CToWUd'></a>"+
             //"</td></tr>"+
             //"<tr>"+"<td style='background:#d97634;color:white;font-weight:400;font-size:12px;padding:1% 1% 1%'>301, 37/2 Royal Plaza, Tikekar Road, Dhantoli,<br>Nagpur - 440012</td>"+
             //"<td style='background:#d97634;color:white;font-weight:400;font-size:12px;padding:1% 1% 1%'>"+
             //"<strong>📞 Phone</strong> +91-9890728799</td>"+
             //"<td style='background:#d97634;color:white;font-weight:400;font-size:12px;padding:1% 1% 1%'>"+
             //"<strong>✉ Email </strong> sam@promaxevents.in </td></tr>"+
             //"</tbody>"+
             //"</table>"+
             //"</td>"+"</tr>"+
             "</tbody>" +
             "</table>" +
             "</td></tr>" +
             "</tbody></table></table></body>" + "";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<header class='clearfix'>");
                sb.Append("<h1 align='center' font-wieght='600'><u><b>Order List</b></u></h1>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<div id='company' class='clearfix'>");
                sb.Append("<div>COMPANY NAME : </span><b>" + company + "</b></div>");
                sb.Append("<div>Order No : # </span><b>" + OrderNumber + "</b></div>");
                sb.Append("<div>Contact :</span> +91-" + Contact + "</div>");
                sb.Append("<div><a href='" + email + "'>EMAIL :  </span><b>" + email + "</b></a></div>");
                sb.Append("</div>");
                sb.Append("<br/>");
                sb.Append("<div id='project' align='left'>");
                sb.Append("<div><span>ORDER DATE (mm-dd-yyyy) :  </span><b> " + OrderDate + "</b></div>");
                sb.Append("<div><span>ISSUE DATE TIME :  </span><b> " + from + "</b></div>");
                sb.Append("<div><span>RETURN DATE TIME :  </span><b> " + To + "</b></div>");
                sb.Append("</div>");
                sb.Append("</header>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<main>");
                sb.Append("<table colspan='4' align ='center' cellpadding='0' cellspacing='0' border='0'> ");
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th bgcolor='black' color='white' padding-bottom='8' width='15%' margin-left='20'><b>Sr No.</b></th>");
                sb.Append("<th bgcolor='black' color='white' padding-bottom='8' width='50'><b>Product Name</b></th>");
                sb.Append("<th bgcolor='black' color='white' padding-bottom='8' width='20%'><b>Quantity</b></th>");
                sb.Append("<th bgcolor='black' color='white' padding-bottom='8' width='15%'><b>Short</b></th>");
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append("<tr></tr>");
                sb.Append("<tbody border='1px'>");
                //Add Items Dynamically From Database --*Start*--//
                sb.Append(OrderItems);
                //Add Items Dynamically From Database --*END*--//
                sb.Append("</tbody>");
                sb.Append("<br/>");
                sb.Append("<tr>");
                sb.Append("<td bgcolor='#AFAAAA' padding-top='8' margin-bottom='8px' align='center' colspan='2' color='black'><strong>Total</strong></td>");
                sb.Append("<td bgcolor='#008a00' padding-top='8' margin-bottom='8px' align='center' colspan='2' color='white'><strong>" + itemsCount + "</strong></td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</main>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<footer>");
                sb.Append("<table  border='0' cellpadding='0' cellspacing='0' colspan='2'>");
                sb.Append("<tr>");
                sb.Append("<td bgcolor='#D97634' color='white' font-size='18px' width='70%' margin-left='20px'><h3><strong><p>  Promax Scientific Developers Pvt. Ltd.</p></strong></h3></td>");
                sb.Append("<td bgcolor='#D97634' color='white' font-size='14px' width='30%' margin-left='20px'><strong><p>  ✉ Email  </strong> sam@promaxevents.in</p></td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td bgcolor='#D97634' color='white' margin-left='20px' width='70%'><p font-size='10px'>  301, 37 / 2 Royal Plaza, Tikekar Road, Dhantoli, Nagpur - 440012</p></td> ");
                sb.Append("<td bgcolor='#D97634' color='white' font-size='14px' margin-left='20px' width='30%'><strong><p>  📞 Phone </strong> +91-9890728799</p></td><br/>");
                sb.Append("</tr>");
                sb.Append("</table> ");
                sb.Append("</footer>");
                //sb.Append(body.ToString().Replace("<body>","").Replace("</body>",""));
                StringReader sr = new StringReader(sb.ToString());

                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    // XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    pdfDoc.Open();

                    htmlparser.Parse(sr);
                    pdfDoc.Close();

                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    Random rn = new Random();
                    string Name = "Order-" + OrderNumber;
                    //  Export(body, Name);
                    MailMessage mm = new MailMessage(fromEmail, toEmail)
                    {
                        Subject = subject,
                        IsBodyHtml = true,
                        Body = header + body + MainBody + FooterBody

                    };
                    mm.Attachments.Add(new Attachment(new MemoryStream(bytes), Name + ".pdf"));
                    // mm.CC.Add(ccMail);
                    smtp.Send(mm);
                }
            }
            catch (Exception exmsg)
            {

            }
        }

        //Send Email To Customer
        [NonAction]
        public void CustomerCopyEmail(string email, string Subject, string company, string OrderNumber, string ParaMsg, string itemList, string OrderItems, int itemsCount, string Contact, string from, string To, string Guid, string OrderDate)
        {
            var fromEmail = new MailAddress("saurabh@promaxevents.in", "Your Order List");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "Kashyap007";
            string subject = Subject;
            string header = "<head>" +
    "<meta charset='UTF-8'>" +
    "<meta name='viewport' content='width=device-width, initial-scale=1.0'>" +
    "<meta http-equiv='X-UA-Compatible' content='ie=edge'>" +
"<style type='text/css'>" +
".body{" +
"font - family: 'Segoe UI', Tahoma, Geneva, Verdana, sans - serif;" +
       " position: absolute;" +
            "align - items: center;" +
       " }" +
        "table.container2 {" +
       " background-repeat: no-repeat;" +
        "background-position: center;" +
       " background-size: 100%;" +
   " }" +

   "table.container3 {" +
      "  background-color: rgb(49, 47, 47);" +
    "background-position: center;" +
      "background-size: 100%;" +
    "}" +
".container {" +
       "width: 100%;" +
        "background-color: White;" +
        "height: 20px;" +
        "font-weight: 200;" +
        "font-size: 12px;" +
        "font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;" +
    "}" +
".imgshift{" +
       " text-align:right;" +
        "align-items: right;" +
        "align-content: right;" +
    "}" +

        ".display{" +
                "text-align:center;" +
                "align-items:center;" +
                "font-size:18px;" +
            "position:relative;" +
                "font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans - serif;" +
            "width:100;" +
            "height:50px;" +
               "background - color:'#EBEBEB';" +
                "font - weight:800;" +
            "padding: 8px;" +
            "}" +
            "</style>" +
    "<title>Promax Cart</title>" +
"</head>" + "";
            string body = "<body>" +
            "<table align='center' cellpadding='10' cellspacing='0' style='width:729px!important;max-width:729px;border:0px solid rgba(0,0,0,0.1);background-color:#f3eeee1c;font-family:Segoe UI', Tahoma, Geneva, Verdana, sans-serif;'>" +
            "<tbody>" +
            "<tr>" +
            "<td>" + "<hr/>" +
            "<table class='container2' align='center' cellpadding='0' cellspacing='0' border='0' style='background-color:rgba(0.84,0.85,0.88,0.2);color:#153643;font-size:16px;padding:2% 2% 1%'>" +
            "<tbody>" +
            "<tr>" + "<td>" +
            "<img class='imgshift' src='http://promaxhdevents.com/wp-content/uploads/2016/07/new-logo02-e1468052055630.png'>" +
            "</td>" + "</tr>" +
            "</tbody>" +
            "</table>" +
            "<hr/>" +
            "</td></tr>" +
            "<tr><td>" +
            "<table align='center' cellpadding='0' cellspacing='0' border='0' style='padding:4px 6px;font-weight:600;color:white;margin-left:3px;font-family:Segoe UI', Tahoma, Geneva, Verdana, sans-serif;'>" +
            "<tbody>" +
            "<tr>" +
            "<td style='background-color:#302e2e8a;font-size:24px;padding-left:10px;height:50px'>" + "<b style='margin-top:8px'>" + company + "</b></td></tr>" +
            "<br>";
            string ParagraphMsg = ParaMsg;
            string MainBodyC = "<tr><td>" +
             "<table colspan='2' style='width:729px;margin-left:3px;margin-top:40px;background-color:#302e2e8a;font-family:Segoe UI', Tahoma, Geneva, Verdana, sans-serif;'>" +
             "<tbody>" +
             "<tr>" + "<td style='font-size:14px;padding-top:10px;font-weight:400;width:60%;color:white;'>🏢 Orgnisation :<b> " + company + "</b></td>" +
             "<td style='font-size:14px;font-weight:400;width:40%;padding-top:10px;color:white;'>⏰From Date :<b> " + from + "</b></td>" +
             "</tr>" +
             "<tr>" +
             "<td style='font-size:14px;font-weight:400;width:60%;padding-top:10px;color:white;'>📋 Order No.<b> # " + OrderNumber + "</b><br/><br/> 🗓️ Order Date:<b> " + OrderDate + " </b> " +
             "</td>" +
             "<td style='font-size:14px;font-weight:400;width:40%;padding-top:10px;color:white;'>⏰ To Date : <b>" + To + "</b>" +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td style='font-size:14px;font-weight:400;width:40%;padding-top:10px;color:white;'>📞 Contact :<b> +91-" + Contact + "</b>" +
             "</td>" +
             "<td style='font-size:12px;font-weight:400;width:60%;padding-top:10px;color:white;'>✉ Email : <b>" + email + "</b>" +
             "</td>" +
             "</tr>" +
             "</tbody>" +
             "</table>" +
             "</td></tr>" +
             "</tbody>" +
             "</table>" +
             "</td></tr>" +
             "</tbody>" +
             "<table align='center' style='font-family:Segoe UI', Tahoma, Geneva, Verdana, sans-serif;'>" +
             "<tbody>" +
             "<tr><td>";
            string MainBody = itemList;
            string FooterBody = "</td>" +
             "</tr>" +
             "<tr>" + "<td>" +
             "<table style='border:0px solid rgba(0,0,0,0.1);color:#153643;font-family:sans-serif;font-size:16px;line-height:22px;padding:20px 6px'>" +
             "<tbody>" +
             "</tbody>" +
             "</table>" +
             "</td>" +
             "</tr>" +
             "<tr><td>" +
             "<table style='width:739px' border='0' cellpadding='0' cellspacing='0'>" +
             "<tbody>" +
             "<tr>" +
             "<td style='background:#d97634;color:white;font-weight:600;font-size:16px;padding:8px;'>Promax Scientific Developers Pvt. Ltd.</td><td style='background:#d97634;padding:8px;'>" + "<a href='https://www.facebook.com/promax.hd.7' target='_blank' data-saferedirecturl='https://www.google.com/url?hl=en&amp;q=https://www.facebook.com/promax.hd.7&amp;source=gmail&amp;ust=1508911257103000&amp;usg=AFQjCNEOC6rjpB-liAj8WGlgiAE33FZ5jw'>" +
             "<img src='https://ci6.googleusercontent.com/proxy/pvX2YraU_BnR_JBocJKVHVaAKfyzxOe_-YUxvkHcW27nTn6pTA0SweVYCZimVRidTHQGBt_keLpt13XY72jbN-OK7zl-1IyjMOZOzKvMDSwDCygZuoDA6fnFU8Z7lCCiqRHvMzKEIHjRano-k9h4Z6Wp-eI12_wp985b_jgewQnVAoxQA9XkrnQgETLY=s0-d-e1-ft#http://images.go.mongodb.com/EloquaImages/clients/MongoDB/%7b1c5f399c-f1d3-4364-be92-c0e4536b42cf%7d_social_icon-04.png' width='40' height='40' style='display:block' class='CToWUd'></a></td><td style='background:#d97634;padding:8px;'>" +
             "<a href='https://twitter.com/promaxhdevents' target='_blank' data-saferedirecturl='https://www.google.com/url?hl=en&amp;q=https://twitter.com/promaxhdevents&amp;source=gmail&amp;ust=1508911257103000&amp;usg=AFQjCNGEwPDH3Dr0SIBYtuP0FOGGM5jDQQ'>" +
             "<img src='https://ci3.googleusercontent.com/proxy/mD7m2RL1crxESqTrztL14mxMTc4ZNq-_DEEQ227dss-mvp2o8gIq0lkLhqYZdpClY7sEp-pERN0d0vZZNXh0HW_pWir0tKHn2rNxqqV89Xt_3C70tdUHGTd8IHL46mGPjaaM3FqbOg28LgqkTlpWpSTiUfMraR9iz9ZxZKJEF3AE05LDm56eNrIqVJ2d=s0-d-e1-ft#http://images.go.mongodb.com/EloquaImages/clients/MongoDB/%7b641c0845-e8fb-4991-bed7-05ccf7fe490f%7d_social_icon-01.png' width='40' height='40' style='display:block' class='CToWUd'></a>" +
             "</td></tr>" +
             "<tr>" + "<td style='background:#d97634;color:white;font-weight:400;font-size:12px;padding:8px;'>301, 37/2 Royal Plaza, Tikekar Road, Dhantoli,<br>Nagpur - 440012</td>" +
             "<td style='background:#d97634;color:white;font-weight:400;font-size:12px;padding:8px;'>" +
             "<strong>📞 Phone</strong> +91-9890728799</td>" +
             "<td style='background:#d97634;color:white;font-weight:400;font-size:12px;padding:8px;'>" +
             "<strong>✉ Email </strong> sam@promaxevents.in </td></tr>" +
             "</tbody>" +
             "</table>" +
             "</td>" + "</tr>" +
             "</tbody>" +
             "</table>" +
             "</td></tr>" +
             "</tbody></table></table></body>" + "";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<header class='clearfix'>");
                sb.Append("<h1 align='center' font-wieght='600'><u><b>Order List</b></u></h1>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<div id='company' class='clearfix'>");
                sb.Append("<div>COMPANY NAME : </span><b>" + company + "</b></div>");
                sb.Append("<div>Order No : # </span><b>" + OrderNumber + "</b></div>");
                sb.Append("<div>Contact :</span> +91-" + Contact + "</div>");
                sb.Append("<div><a href='" + email + "'>EMAIL :  </span><b>" + email + "</b></a></div>");
                sb.Append("</div>");
                sb.Append("<br/>");
                sb.Append("<div id='project' align='left'>");
                sb.Append("<div><span>ORDER DATE (mm-dd-yyyy) :  </span><b> " + OrderDate + "</b></div>");
                sb.Append("<div><span>ISSUE DATE TIME :  </span><b> " + from + "</b></div>");
                sb.Append("<div><span>RETURN DATE TIME :  </span><b> " + To + "</b></div>");
                sb.Append("</div>");
                sb.Append("</header>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<main>");
                sb.Append("<table colspan='3' align ='center' cellpadding='0' cellspacing='0' border='0'> ");
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th bgcolor='black' color='white' padding-bottom='8' width='25%' margin-left='20'><b>Sr No.</b></th>");
                sb.Append("<th bgcolor='black' color='white' padding-bottom='8' width='50%'><b>Product Name</b></th>");
                sb.Append("<th bgcolor='black' color='white' padding-bottom='8' width='25%'><b>Quantity</b></th>");
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append("<tr></tr>");
                sb.Append("<tbody border='1px'>");
                //Add Items Dynamically From Database --*Start*--//
                sb.Append(OrderItems);
                //Add Items Dynamically From Database --*END*--//
                sb.Append("</tbody>");
                sb.Append("<br/>");
                sb.Append("<tr>");
                sb.Append("<td bgcolor='#AFAAAA' padding-top='8' margin-bottom='8px' align='center' colspan='2' color='black'><strong>Total</strong></td>");
                sb.Append("<td bgcolor='#008a00' padding-top='8' margin-bottom='8px' align='center' color='white'><strong>" + itemsCount + "</strong></td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</main>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<footer>");
                sb.Append("<table  border='0' cellpadding='0' cellspacing='0' colspan='2'>");
                sb.Append("<tr>");
                sb.Append("<td bgcolor='#D97634' color='white' font-size='18px' width='70%' margin-left='20px'><h3><strong><p>  Promax Scientific Developers Pvt. Ltd.</p></strong></h3></td>");
                sb.Append("<td bgcolor='#D97634' color='white' font-size='14px' width='30%' margin-left='20px'><strong><p>  ✉ Email  </strong> sam@promaxevents.in</p></td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td bgcolor='#D97634' color='white' margin-left='20px' width='70%'><p font-size='10px'>  301, 37 / 2 Royal Plaza, Tikekar Road, Dhantoli, Nagpur - 440012</p></td> ");
                sb.Append("<td bgcolor='#D97634' color='white' font-size='14px' margin-left='20px' width='30%'><strong><p>  📞 Phone </strong> +91-9890728799</p></td><br/>");
                sb.Append("</tr>");
                sb.Append("</table> ");
                sb.Append("</footer>");
                //sb.Append(body.ToString().Replace("<body>","").Replace("</body>",""));
                StringReader sr = new StringReader(sb.ToString());

                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    // XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    pdfDoc.Open();

                    htmlparser.Parse(sr);
                    pdfDoc.Close();

                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    Random rn = new Random();
                    string Name = "Order-" + OrderNumber;
                    //  Export(body, Name);
                    MailMessage mm = new MailMessage(fromEmail, toEmail)
                    {
                        Subject = subject,
                        IsBodyHtml = true,
                        Body = header + body + ParagraphMsg + MainBodyC + MainBody + FooterBody
                    };
                    mm.Attachments.Add(new Attachment(new MemoryStream(bytes), Name + ".pdf"));

                    smtp.Send(mm);
                }
            }
            catch (Exception exmsg)
            {

            }
        }

        //Send Email to WareHouse
        [NonAction]
        public void SendEmailToWareHouse(string email, string Subject, string company, string OrderNumber, string itemList, string OrderItems, int itemsCount, string Contact, string from, string To, string Guid, string OrderDate)
        {
            var fromEmail = new MailAddress("saurabh@promaxevents.in", "New Order List");
            var toEmail = new MailAddress("saurabh@promaxevents.in");
            var fromEmailPassword = "Kashyap007";
            string subject = Subject;
            string header = "<head>" +
    "<meta charset='UTF-8'>" +
    "<meta name='viewport' content='width=device-width, initial-scale=1.0'>" +
    "<meta http-equiv='X-UA-Compatible' content='ie=edge'>" +
"<style type='text/css'>" +
".body{" +
"font - family: 'Segoe UI', Tahoma, Geneva, Verdana, sans - serif;" +
       " position: absolute;" +
            "align - items: center;" +
       " }" +
        "table.container2 {" +
       " background-repeat: no-repeat;" +
        "background-position: center;" +
       " background-size: 100%;" +
   " }" +

   "table.container3 {" +
      "  background-color: rgb(49, 47, 47);" +
    "background-position: center;" +
      "background-size: 100%;" +
    "}" +
".container {" +
       "width: 100%;" +
        "background-color: White;" +
        "height: 20px;" +
        "font-weight: 200;" +
        "font-size: 12px;" +
        "font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;" +
    "}" +
".imgshift{" +
       " text-align:right;" +
        "align-items: right;" +
        "align-content: right;" +
    "}" +
"</style>" +
    "<title>Promax Cart</title>" +
"</head>" + "";
            string body = "<body>" +
            "<table align='center' cellpadding='10' cellspacing='0' style='width:729px!important;max-width:729px;border:0px solid rgba(0,0,0,0.1);background-color:#f3eeee1c;font-family:Segoe UI', Tahoma, Geneva, Verdana, sans-serif;'>" +
            "<tbody>" +
            "<tr>" +
            "<td>" + "<hr/>" +
            "<table class='container2' align='center' cellpadding='0' cellspacing='0' border='0' style='background-color:rgba(0.84,0.85,0.88,0.2);color:#153643;font-size:16px;padding:2% 2% 1%'>" +
            "<tbody>" +
            "<tr>" + "<td>" +
            "<img class='imgshift' src='http://promaxhdevents.com/wp-content/uploads/2016/07/new-logo02-e1468052055630.png'/>" +
            "</td>" + "</tr>" +
            "</tbody>" +
            "</table>" +
            "<hr/>" +
            "</td></tr>" +
            "<tr><td>" +
            "<table align='center' cellpadding='0' cellspacing='0' border='0' style='padding:4px 6px;font-weight:600;color:white;margin-left:3px;font-family:Segoe UI', Tahoma, Geneva, Verdana, sans-serif;'>" +
            "<tbody>" +
            "<tr>" +
            "<td style='background-color:#302e2e8a;font-size:24px;padding-left:10px;height:50px'>" + "<b style='margin-top:8px'>Dear Promax Scientific Developers Pvt.Ltd.</b></td></tr>" +
            "<tr><td>" +
            "<table colspan='2' style='width:729px;margin-left:3px;margin-top:40px;background-color:#302e2e8a;font-family:Segoe UI', Tahoma, Geneva, Verdana, sans-serif;'>" +
            "<tbody>" +
            "<tr>" + "<td style='font-size:14px;padding-top:10px;font-weight:400;width:60%;color:white;'>🏢 Orgnisation :<b> " + company + "</b></td>" +
            "<td style='font-size:14px;font-weight:400;width:40%;padding-top:10px;color:white;'>⏰From Date :<b> " + from + "</b></td>" +
            "</tr>" +
            "<tr>" +
            "<td style='font-size:14px;font-weight:400;width:60%;padding-top:10px;color:white;'>📋 Order No.<b> # " + OrderNumber + "</b><br/><br/> 🗓️ Order Date: <b>" + OrderDate + "</b>"+
            "</td>" +
            "<td style='font-size:14px;font-weight:400;width:40%;padding-top:10px;color:white;'>⏰ To Date : <b>" + To + "</b>" +
            "</td>" +
            "</tr>" +
            "<tr>" +
            "<td style='font-size:14px;font-weight:400;width:40%;padding-top:10px;color:white;'>📞 Contact :<b> +91-" + Contact + "</b>" +
            "</td>" +
            "<td style='font-size:12px;font-weight:400;width:60%;padding-top:10px;color:white;'>✉ Email : <b>" + email + "</b>" +
            "</td>" +
            "</tr>" +
            "</tbody>" +
            "</table>" +
            "</td></tr>" +
            "</tbody>" +
            "</table>" +
            "</td></tr>" +
            "</tbody>" +
            "<table align='center' style='font-family:Segoe UI', Tahoma, Geneva, Verdana, sans-serif;'>" +
            "<tbody>" +
            "<tr>" + "<td>" +
            "<table align='center' cellpadding='0' cellspacing='0' border='0'>" +
            "<tbody><tr><td style='font-size:30px;font-weight:600;'><h3>🚚 Order List</h3>" +
            "</td></tr>" +
            "</tbody>" +
            "</table>" +
            "</td>" +
            "</tr>" +
            "<tr>" +
            "<td>" +
            "<table align='center' cellpadding='0' cellspacing='0' border='0'>" +
            "<tbody>" +
            "<tr>" +
            "<td style='margin-top:2px;padding:1px 2px'>" + "";
            string MainBody = itemList;
            string FooterBody = "</td>" +
             "</tr>" +
             "<br/>" +
             "<br/>" +
             //"<tr><td>" +
             //"<table style='width:729px' border='0' cellpadding='0' cellspacing='0'>" +
             //"<tbody>" +
             //"<tr>" +
             //"<td style='background:#d97634;color:white;font-weight:600;font-size:16px;padding:1% 1% 1%'>Promax Scientific Developers Pvt. Ltd.</td><td style='background:#d97634;padding:0% 0% -5%'>" + "<a href='https://www.facebook.com/promax.hd.7' target='_blank' data-saferedirecturl='https://www.google.com/url?hl=en&amp;q=https://www.facebook.com/promax.hd.7&amp;source=gmail&amp;ust=1508911257103000&amp;usg=AFQjCNEOC6rjpB-liAj8WGlgiAE33FZ5jw'>" +
             //"<img src='https://ci6.googleusercontent.com/proxy/pvX2YraU_BnR_JBocJKVHVaAKfyzxOe_-YUxvkHcW27nTn6pTA0SweVYCZimVRidTHQGBt_keLpt13XY72jbN-OK7zl-1IyjMOZOzKvMDSwDCygZuoDA6fnFU8Z7lCCiqRHvMzKEIHjRano-k9h4Z6Wp-eI12_wp985b_jgewQnVAoxQA9XkrnQgETLY=s0-d-e1-ft#http://images.go.mongodb.com/EloquaImages/clients/MongoDB/%7b1c5f399c-f1d3-4364-be92-c0e4536b42cf%7d_social_icon-04.png' width='40' height='40' style='display:block' class='CToWUd'></a></td><td style='background:#d97634;padding:1% 1% 1%'>" +
             //"<a href='https://twitter.com/promaxhdevents' target='_blank' data-saferedirecturl='https://www.google.com/url?hl=en&amp;q=https://twitter.com/promaxhdevents&amp;source=gmail&amp;ust=1508911257103000&amp;usg=AFQjCNGEwPDH3Dr0SIBYtuP0FOGGM5jDQQ'>" +
             //"<img src='https://ci3.googleusercontent.com/proxy/mD7m2RL1crxESqTrztL14mxMTc4ZNq-_DEEQ227dss-mvp2o8gIq0lkLhqYZdpClY7sEp-pERN0d0vZZNXh0HW_pWir0tKHn2rNxqqV89Xt_3C70tdUHGTd8IHL46mGPjaaM3FqbOg28LgqkTlpWpSTiUfMraR9iz9ZxZKJEF3AE05LDm56eNrIqVJ2d=s0-d-e1-ft#http://images.go.mongodb.com/EloquaImages/clients/MongoDB/%7b641c0845-e8fb-4991-bed7-05ccf7fe490f%7d_social_icon-01.png' width='40' height='40' style='display:block' class='CToWUd'></a>" +
             //"</td></tr>" +
             //"<tr>" + "<td style='background:#d97634;color:white;font-weight:400;font-size:12px;padding:1% 1% 1%'>301, 37/2 Royal Plaza, Tikekar Road, Dhantoli,<br>Nagpur - 440012</td>" +
             //"<td style='background:#d97634;color:white;font-weight:400;font-size:12px;padding:1% 1% 1%'>" +
             //"<strong>📞 Phone</strong> +91-9890728799</td>" +
             //"<td style='background:#d97634;color:white;font-weight:400;font-size:12px;padding:1% 1% 1%'>" +
             //"<strong>✉ Email </strong> sam@promaxevents.in </td></tr>" +
             //"</tbody>" +
             //"</table>" +
             //"</td>" + "</tr>" +
             "</tbody>" +
             "</table>" +
             "</td></tr>" +
             "</tbody></table></table></body>" + "";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<header class='clearfix'>");
                sb.Append("<h1 align='center' font-wieght='600'><u><b>Order List</b></u></h1>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<div id='company' class='clearfix'>");
                sb.Append("<div>COMPANY NAME : </span><b>" + company + "</b></div>");
                sb.Append("<div>Order No : # </span><b>" + OrderNumber + "</b></div>");
                sb.Append("<div>Contact :</span> +91-" + Contact + "</div>");
                sb.Append("<div><a href='" + email + "'>EMAIL :  </span><b>" + email + "</b></a></div>");
                sb.Append("</div>");
                sb.Append("<br/>");
                sb.Append("<div id='project' align='left'>");
                sb.Append("<div><span>ORDER DATE (mm-dd-yyyy) :  </span><b> " + OrderDate + "</b></div>");
                sb.Append("<div><span>ISSUE DATE TIME :  </span><b> " + from + "</b></div>");
                sb.Append("<div><span>RETURN DATE TIME :  </span><b> " + To + "</b></div>");
                sb.Append("</div>");
                sb.Append("</header>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<main>");
                sb.Append("<table colspan='3' align ='center' cellpadding='0' cellspacing='0' border='0'> ");
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th bgcolor='black' color='white' padding-bottom='8' width='15%' margin-left='20'><b>Sr No.</b></th>");
                sb.Append("<th bgcolor='black' color='white' padding-bottom='8' width='55%'><b>Product Name</b></th>");
                sb.Append("<th bgcolor='black' color='white' padding-bottom='8' width='15%'><b>Quantity</b></th>");
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append("<tr></tr>");
                sb.Append("<tbody border='1px'>");
                //Add Items Dynamically From Database --*Start*--//
                sb.Append(OrderItems);
                //Add Items Dynamically From Database --*END*--//
                sb.Append("</tbody>");
                sb.Append("<br/>");
                sb.Append("<tr>");
                sb.Append("<td bgcolor='#AFAAAA' padding-top='8' margin-bottom='8px' align='center' colspan='2' color='black'><strong>Total</strong></td>");
                sb.Append("<td bgcolor='#008a00' padding-top='8' margin-bottom='8px' align='center' color='white'><strong>" + itemsCount + "</strong></td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</main>");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("<footer>");
                sb.Append("<table  border='0' cellpadding='0' cellspacing='0' colspan='2'>");
                sb.Append("<tr>");
                sb.Append("<td bgcolor='#D97634' color='white' font-size='18px' width='70%' margin-left='20px'><h3><strong><p>  Promax Scientific Developers Pvt. Ltd.</p></strong></h3></td>");
                sb.Append("<td bgcolor='#D97634' color='white' font-size='14px' width='30%' margin-left='20px'><strong><p>  ✉ Email  </strong> sam@promaxevents.in</p></td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td bgcolor='#D97634' color='white' margin-left='20px' width='70%'><p font-size='10px'>  301, 37 / 2 Royal Plaza, Tikekar Road, Dhantoli, Nagpur - 440012</p></td> ");
                sb.Append("<td bgcolor='#D97634' color='white' font-size='14px' margin-left='20px' width='30%'><strong><p>  📞 Phone </strong> +91-9890728799</p></td><br/>");
                sb.Append("</tr>");
                sb.Append("</table> ");
                sb.Append("</footer>");

                //sb.Append(body.ToString().Replace("<body>","").Replace("</body>",""));
                StringReader sr = new StringReader(sb.ToString());

                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    // XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    pdfDoc.Open();

                    htmlparser.Parse(sr);
                    pdfDoc.Close();

                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    Random rn = new Random();
                    string Name = "Order-" + OrderNumber;
                    //  Export(body, Name);
                    MailMessage mm = new MailMessage(fromEmail, toEmail)
                    {
                        Subject = subject,
                        IsBodyHtml = true,
                        Body = header + body + MainBody + FooterBody
                    };
                    mm.Attachments.Add(new Attachment(new MemoryStream(bytes), Name + ".pdf"));

                    smtp.Send(mm);
                }
            }
            catch (Exception exmsg)
            {

            }
        }

    }
}