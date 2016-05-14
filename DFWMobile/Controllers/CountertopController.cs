using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DFWMobile.Controllers
{
    public class CountertopController : Controller
    {
        // GET: Countertop
        public ActionResult Index()
        {
            return View();
        }

        // GET: Countertop/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Countertop/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Countertop/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Countertop/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Countertop/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Countertop/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Countertop/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
