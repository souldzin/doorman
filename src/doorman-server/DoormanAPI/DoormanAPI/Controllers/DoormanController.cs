using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoormanAPI.Hub;
using DoormanAPI.Models;
using DoormanAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DoormanAPI.Controllers
{
    [Route("api/doorman/")]
    public class DoormanController : Controller
    {
	    private readonly IHubContext<DoormanHub> _context;
		private readonly IDoormanService _doormanService;

	    public DoormanController(IDoormanService doormanService, IHubContext<DoormanHub> context)
	    {
		    _doormanService = doormanService;
		    _context = context;
	    }

	    [HttpGet("rooms", Name = "GetRooms")]
        public IActionResult GetRooms()
        {
	        try
	        {
		        var result = _doormanService.GetRooms();
		        return Ok(result);
	        }
	        catch (Exception ex)
	        {
		        return StatusCode(500, "A problem happening while handling your request");
	        }
        }

	    [HttpGet("{snapId}", Name = "GetRoomSnapshot")]
	    public IActionResult GetRoomSnapshot(int snapId)
	    {
		    try
		    {
			    var result = _doormanService.GetRoomOccupancySnapshotById(snapId);

			    if (result == null)
			    {
				    return NotFound();
			    }

			    return Ok(result);
		    }
		    catch (Exception ex)
		    {
			    return StatusCode(500, "A problem happening while handling your request");
		    }
	    }

	    [HttpGet]
	    public IActionResult Get()
	    {
		    var result = _doormanService.GetAll();
		   
		    return Ok(result);
	    }

		[HttpPost]
        public IActionResult Post([FromBody]RoomOccupancySnapshotVM model)
        {
	        if (model == null)
	        {
		        return BadRequest();
	        }

	        var result = _doormanService.SaveRoomOccupancySnapshot(model);

	        if (result == null)
	        {
		        return NotFound();
	        }

			//Components for providing real-time bi-directional communication across the Web
			_context.Clients.All.InvokeAsync("Broadcast", result);

	        return CreatedAtAction("GetRoomSnapshot", new {snapId = result.RoomOccupancySnapshotId}, null);
        }
    }
}
