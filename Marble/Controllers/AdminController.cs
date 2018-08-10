using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Marble.Models;

namespace Marble.Controllers
{
    public class AdminController : Controller
    {
        DBMARBLEEntities db = new DBMARBLEEntities();
        // GET: Admin
        
        public ActionResult AdminIndex()
        {            
            return View();
        }
        public ActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddProduct(tbl_products tblprod, HttpPostedFileBase[] pic, string code)
        {
            try
            {
                if (db.tbl_products.Any(p => p.code == code))
                {
                    Response.Write("<script>alert('Your code is already exists try another one')</script>");
                }
                else
                {
                        tbl_pic tblpic = new tbl_pic();
                        //ensure model state is valid
                        if (ModelState.IsValid)
                        {
                            //iterating through multiple file collection   
                            string[] picarray = new string[4];
                            int i = 0;
                            foreach (HttpPostedFileBase file in pic)
                            {
                                //Checking file is available to save.  
                                if (file != null)
                                {
                                    string randomname = Path.GetRandomFileName();
                                    var InputFileName = randomname + Path.GetFileName(file.FileName);
                                    var ServerSavePath = Path.Combine(Server.MapPath("~/Content/productimg/") + InputFileName);
                                    //Save file to server folder  
                                    file.SaveAs(ServerSavePath);
                                    picarray[i] = InputFileName;
                                    i++;
                                }
                            }
                            db.tbl_products.Add(tblprod);
                            db.SaveChanges();
                            tblpic.pic1 = picarray[0];
                            tblpic.pic2 = picarray[1];
                            tblpic.pic3 = picarray[2];
                            tblpic.pic4 = picarray[3];
                            tblpic.subcategory = tblprod.subcategory;
                            tblpic.code = tblprod.subcategory;
                            db.tbl_pic.Add(tblpic);
                            db.SaveChanges();
                            //assigning file uploaded status to ViewBag for showing message to user. 
                            ViewBag.msg = " Items uploaded successfully.";
                        }
                    else
                    {
                        ViewBag.msg = "Please upload only 4 images";
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.msg = "something went wrong please try again later";
            }
            return View();
        }
        [HttpGet]
        public ActionResult productin()
        {
            List<tbl_products> tblprod = db.tbl_products.ToList();
            ViewData["prodname"] = tblprod;
            return View();
        }
        [HttpPost]
        public ActionResult productin(tbl_in tblin)
        {
            try
            {
                tblin.itemid = Convert.ToInt32(TempData["itemid"].ToString());
                db.tbl_in.Add(tblin);
                db.SaveChanges();
                tbl_products tblprod = db.tbl_products.SingleOrDefault(p => p.productid == tblin.itemid);
                tblprod.quantity += tblin.quantity;
                db.SaveChanges();
                ViewBag.msg = "Operation Successfull";
            }
            catch (Exception e)
            {
                ViewBag.msg = "something went wrong" + e;
            }
            return View();
        }
        [HttpGet]
        public ActionResult productout()
        {
            List<tbl_products> tblprod = db.tbl_products.ToList();
            ViewData["prodname"] = tblprod;
            return View();
        }
        [HttpPost]
        public ActionResult productout(string billno)
        {
            //try
            //{
            //    tblout.itemid = Convert.ToInt32(TempData["itemid"].ToString());
            //    db.tbl_out.Add(tblout);
            //    db.SaveChanges();
            //    tbl_products tblprod = db.tbl_products.SingleOrDefault(p => p.productid == tblout.itemid);
            //    tblprod.quantity -= tblout.quantity;
            //    db.SaveChanges();
            //    ViewBag.msg = "Operation successfull.";
            //}
            //catch (Exception e)
            //{
            //    ViewBag.msg = "something went wrong..";
            //}
            var billpdf = db.tbl_out.Where(p => p.billno == billno).ToList();
            TempData["bill"] = billpdf;
            return RedirectToAction("billpage");
        }
        public ActionResult billpage()
        {
            ViewData["bill"] = TempData["bill"];
            return View();
        }
        public ActionResult getitemname(string size)//ajax method for load item name
        {
            var list = db.tbl_products.Where(p => p.size == size).Select(p => p.name).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult itemdesc(string name,string size)
        {
            string desc = "";
            tbl_products lst = db.tbl_products.SingleOrDefault(m => m.name == name && m.size == size);
            if (lst!=null)
            {
                desc = lst.category + "/" + lst.subcategory + "/" + lst.name + "/" + lst.code;
            }
            TempData["itemid"] = lst.productid;
            return Json(desc, JsonRequestBehavior.AllowGet);
        }
        public ActionResult checkquantity(float quantity, string name, string size)
        {
            bool msg=true;
            float dbqty=1;
            tbl_products lst = db.tbl_products.SingleOrDefault(p=>p.name == name && p.size == size);
            if (lst!=null)
            {
                if (lst.quantity < quantity)
                {
                    msg = false;
                }
                else
                {
                    msg = true;
                }
                dbqty = (float)lst.quantity;
            }
            return Json(new { msg ,dbqty} , JsonRequestBehavior.AllowGet);
        }
        //ajax method for adding item
        public ActionResult additem(tbl_out tblout,string itemname,string itemdescription,float quantity,string size,string billno,string date,string partyname,string mobile)
        {
            bool res = false;
            try
            {
                tbl_out dbBillno = db.tbl_out.SingleOrDefault(p => p.billno == billno);
                if (dbBillno==null)
                {
                        tblout.itemid = Convert.ToInt32(TempData["itemid"].ToString());
                        db.tbl_out.Add(tblout);
                        db.SaveChanges();
                        tbl_products tblprod = db.tbl_products.SingleOrDefault(p => p.productid == tblout.itemid);
                        tblprod.quantity -= tblout.quantity;
                        db.SaveChanges();
                        ViewBag.msg = "Operation successfull.server";
                        res = true;
                }
            }
            catch (Exception e)
            {
                ViewBag.msg = "something went wrong server";
            }
            return Json(/*new { Result = "Success", Message = "Saved Successfully" }*/res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult viewmsg()
        {
            List<tbl_contactus> list = db.tbl_contactus.ToList();
            ViewData["msg"] = list;
            return View();
        }
        public ActionResult inrecord()
        {
            List<tbl_in> list = db.tbl_in.ToList();
            ViewData["in"] = list;
            return View();
        }
        public ActionResult outrecord()
        {
            List<tbl_out> list = db.tbl_out.ToList();
            ViewData["out"] = list;
            return View();
        }
        [HttpGet]
        public ActionResult loadproduct(string subcat)
        {
            return View();
        }
        public ActionResult updatestock(int id)
        {
            return View();
        }
        public ActionResult delstock(int id)
        {
            tbl_products prod = db.tbl_products.SingleOrDefault(p => p.productid == id);
            db.tbl_products.Remove(prod);
            db.SaveChanges();
            Response.Redirect("viewproducts");
            return View();
        }
        public ActionResult outdel(int id)
        {
            tbl_out tbout = db.tbl_out.SingleOrDefault(p => p.id == id);
            db.tbl_out.Remove(tbout);
            db.SaveChanges();
            Response.Redirect("outrecord");
            return View();
        }
        public ActionResult indel(int id)
        {
            tbl_in tbin = db.tbl_in.SingleOrDefault(p => p.id == id);
            db.tbl_in.Remove(tbin);
            db.SaveChanges();
            Response.Redirect("inrecord");
            return View();
        }
        public ActionResult viewproducts()
        {
            List<tbl_products> list = db.tbl_products.ToList();
            ViewData["allprod"] = list;
            return View();
        }
        public ActionResult logout()
        {
            Session.Abandon();
            return RedirectToAction("../Home/Index");
        }
    }
}