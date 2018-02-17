using Microsoft.AspNetCore.Mvc;

namespace DoormanUI.Controllers
{
	public class DoormanController : Controller
	{
		// GET
		public IActionResult Index()
		{
			return View();
		}
	}
}