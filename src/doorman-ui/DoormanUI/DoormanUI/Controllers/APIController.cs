using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoormanUI.Controllers
{
    public class APIController : Controller
    {
        // GET: API
        public ActionResult GetRealTime()
        {
            Random rnd = new Random();
            int size = rnd.Next(52);
            return Json(new {val= size});
        }
    }
}