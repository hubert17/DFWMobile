using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DFWMobile.Models;
using DFWMobile.Models.ViewModels;

namespace DFWMobile.Controllers
{
    public class SlabPromoController : Controller
    {
        [HttpGet]
        public ActionResult Index(int limit = -1)
        {
            DFWRepository dfwRep = new DFWRepository();

            HomePageVM homePageVM = new HomePageVM();

            if (limit != -1)
            {
                homePageVM.slabPromo = dfwRep.getSlabPromo(limit);
                homePageVM.slabPromoLabel = dfwRep.getSlabPromoLabel();
                return PartialView("_SlabPromo", homePageVM);
            }
            else
            {
                homePageVM.slabPromo = dfwRep.getSlabPromo(0);
                homePageVM.slabPromoLabel = dfwRep.getSlabPromoLabel();
                homePageVM.slabPromoLabel.ShownAll = true;
                return View(homePageVM);
            }
        }
    }
}
