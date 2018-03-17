using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Doorman.UI.Web.Models;

namespace Doorman.UI.Web.Controllers
{
    [Route("/dashboard")]
    public class DashboardController : Controller
    {
        [Route("realtime")]
        public IActionResult RealTime() 
        {
            return View();
        }

        [Route("historic")]
        public IActionResult Historic()
        {
            return View();
        }
    }
}