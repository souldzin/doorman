using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doorman.Master.Hub;
using Doorman.Master.Models;
using Doorman.Master.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Doorman.Master.Controllers
{
    [Route("api/doorman/")]
    public class DoormanController : Controller
    {
		private readonly IDoormanService _doormanService;
	    private readonly string _clientId;
	    private readonly string _clientSecret;

	    public DoormanController(IDoormanService doormanService)
	    {
		    _doormanService = doormanService;
		    _clientId = Startup.Configuration["DoormanConfig:client_id"];
		    _clientSecret = Startup.Configuration["DoormanConfig:client_secret"];
	    }

		#region Room Monitor APIs
		[HttpPost("room/current/snapshot")]
        public IActionResult Post([FromBody]PostRoomOccupancySnapshotVM model) //Submit an occupancy snapshot for the authorized room
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

			_doormanService.SendBroadcast(model.RoomId);

	        return Ok(result);
        }

	    [HttpPost("room")]
	    public IActionResult PostRoom([FromQuery] string name, [FromBody]ClientConfigVM model) //Register a new room to the Doorman Master
	    {
		    if (model == null || !model.ClientId.Equals(_clientId, StringComparison.OrdinalIgnoreCase) ||
		        !model.ClientSecret.Equals(_clientSecret, StringComparison.OrdinalIgnoreCase))
		    {
			    return BadRequest();
		    }

		    var result = _doormanService.RegisterRoom(name, model);

		    if (result == null)
		    {
			    return NotFound();
		    }

		    return CreatedAtAction("GetRoom", new { roomId = result.RoomId }, result);
	    }

		#endregion

		#region Room UI APIs
	    [HttpGet("room", Name = "GetRoom")]
	    public IActionResult GetRoom(int roomId) //Get information for the room with the given ID
	    {
		    var result = _doormanService.GetRoom(roomId);

		    if (result == null)
		    {
			    return NotFound();
		    }

		    return Ok(result);
	    }

		[HttpGet("room/{roomId}/historicTrend", Name = "GetHistoricTrend")]
		public IActionResult GetHistoricTrend(int roomId, [FromQuery]DateTime start, [FromQuery]DateTime end) //Get information for the room within the date range no more than 30 data points
		{
			var room  = _doormanService.GetRoom(roomId);

			if (room == null)
			{
				return NotFound();
			}

			var result = _doormanService.GetHistoricTrends(roomId, start, end);

			return Ok(result);
		}

	    [HttpGet("room/{roomId}/recentTrend", Name = "GetRecentTrend")]
	    public IActionResult GetRecentTrend(int roomId, int seconds) //Get information for the room within the given seconds no more than 30 data points
	    {
		    var room = _doormanService.GetRoom(roomId);

		    if (room == null)
		    {
			    return NotFound();
		    }

		    var result = _doormanService.GetRecentTrends(roomId, seconds);

		    return Ok(result);
	    }

	    [HttpGet("room/{roomId}/stats", Name = "GetStats")]
	    public IActionResult GetStats(int roomId, [FromQuery]DateTime start, [FromQuery]DateTime end) //For the room calculate some statistical metrics of the occupancy between date range
	    {
		    var room = _doormanService.GetRoom(roomId);

		    if (room == null)
		    {
			    return NotFound();
		    }

		    var result = _doormanService.GetStats(roomId, start, end);

		    return Ok(result);
	    }
		#endregion
	}
}
