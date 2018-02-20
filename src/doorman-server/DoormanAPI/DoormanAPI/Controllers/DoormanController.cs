using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoormanAPI.Models;
using DoormanAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoormanAPI.Controllers
{
    [Route("api/doorman")]
    public class DoormanController : Controller
    {
	    private readonly IDoormanService _doormanService;

	    public DoormanController(IDoormanService doormanService)
	    {
		    _doormanService = doormanService;
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

	    [HttpGet("snapshot/{snapId}", Name = "GetRoomSnapshot")]
	    public IActionResult GetRoomSnapshot([FromQuery] int snapId)
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
		        NotFound();
	        }

	        return CreatedAtAction("GetRoomSnapshot", new {snapId = result});
        }
    }
}
