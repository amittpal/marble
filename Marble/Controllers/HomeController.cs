using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marble.Models;

namespace Marble.Controllers
{
    public class HomeController : Controller
    {
        DBMARBLEEntities db = new DBMARBLEEntities();
        // GET: Home
        public ActionResult Index()
        {
            List<tbl_pic> list1 = db.tbl_pic.OrderByDescending(p => p.picid).Take(9).Distinct().ToList();
            ViewData["prodpics"] = list1;
            List<tbl_products> prodmarbles = db.tbl_products.Where(p=>p.category=="Marble").Distinct().ToList();
            ViewData["prodmarbles"] = prodmarbles;
            List<tbl_products> prodtiles = db.tbl_products.Where(p => p.category == "Tiles").Distinct().ToList();
            ViewData["prodtiles"] = prodtiles;
            List<tbl_products> prodstones = db.tbl_products.Where(p => p.category == "Stones").Distinct().ToList();
            ViewData["prodstones"] = prodstones;
            return View();
        }
        [HttpGet]
        public ActionResult loadproduct(int id)
        {
            List<tbl_products> prodmarbles = db.tbl_products.Where(p => p.category == "Marble").ToList();
            ViewData["prodmarbles"] = prodmarbles;
            List<tbl_products> prodtiles = db.tbl_products.Where(p => p.category == "Tiles").ToList();
            ViewData["prodtiles"] = prodtiles;
            List<tbl_products> prodstones = db.tbl_products.Where(p => p.category == "Stones").ToList();
            ViewData["prodstones"] = prodstones;

            tbl_pic prodpic = db.tbl_pic.SingleOrDefault(p => p.picid == id);
            ViewData["prodpic"] = prodpic;
            tbl_products proddetail = db.tbl_products.SingleOrDefault(u => u.productid == id);
            ViewData["proddetail"] = proddetail;
            return View();
        }
        [HttpGet]
        public ActionResult contactus()
        {
            List<tbl_products> prodmarbles = db.tbl_products.Where(p => p.category == "Marble").ToList();
            ViewData["prodmarbles"] = prodmarbles;
            List<tbl_products> prodtiles = db.tbl_products.Where(p => p.category == "Tiles").ToList();
            ViewData["prodtiles"] = prodtiles;
            List<tbl_products> prodstones = db.tbl_products.Where(p => p.category == "Stones").ToList();
            ViewData["prodstones"] = prodstones;
            return View();
        }
        [HttpPost]
        public ActionResult contactus(tbl_contactus tblcontact)
        {
            try
            {
                db.tbl_contactus.Add(tblcontact);
                db.SaveChanges();
                ViewBag.msg = "Thanks for your Enquiry";
                //Response.Write("<script>alert('Thanks for your Enquiry')</script>");
            }
            catch (Exception e)
            {
                ViewBag.msg = "Something went wrong";
            }
            return View();
        }
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult login(tbl_login tbllogin)
        {
            try
            {
                tbl_login lg = db.tbl_login.SingleOrDefault(p => p.id == tbllogin.id&p.password==tbllogin.password);
                if (lg!=null)
                {
                    Session["loginid"] = lg.id;
                    return RedirectToAction("../Admin/AdminIndex");
                }
                else
                {
                    ViewBag.msg = "Invalid Username Or Password";
                }
            }
            catch (Exception c)
            {
                ViewBag.msg = "something went wrong";
            }
            return View();
        }
    }
}