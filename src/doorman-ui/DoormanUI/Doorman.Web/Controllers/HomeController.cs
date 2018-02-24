using Doorman.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Doorman.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Historic()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //public PartialViewResult _historicData(string StartDate = "", string EndDate = "", int ChartType = 1)
        //{
        //    var model = new HistoricData(StartDate, EndDate, ChartType);

        //    return PartialView(model);
        //}
    }
}