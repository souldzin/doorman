using Doorman.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Doorman.Web.Controllers
{
    public class APIController : Controller
    {
        // GET: API
        public ActionResult GetRealTime()
        {
            Random rnd = new Random();
            int size = rnd.Next(52);
            return Json(new {val= size},JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetHistoricModel(string StartDate = "", string EndDate = "", int ChartType = 1)
        {
            var model = new HistoricData(StartDate, EndDate, ChartType);
            var page = RenderPartialViewToString("../home/_historicData", model);
            var ChartData = new HistoricChartData(StartDate, EndDate, ChartType);
            return Json(new { data = ChartData.Series, categories = ChartData.Categories, page = page }, JsonRequestBehavior.AllowGet);
        }

        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}