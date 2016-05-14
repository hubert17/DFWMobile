using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DFWMobile.Models;
using DFWMobile.Models.ViewModels;

namespace DFWMobile.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DFWRepository dfwRep = new DFWRepository();

            HomePageVM homePageVM = new HomePageVM();
            homePageVM.slabPromo = dfwRep.getSlabPromo(6);
            homePageVM.slabPromoLabel = dfwRep.getSlabPromoLabel();

            return View(homePageVM);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult pdftest()
        {
            return View("http://www.granitesouthlake.com/m/online-quote/QUOTEWG_DFW-Wholesale-Granite.aspx?OnlineQuoteID=4635");
        }

    }
}