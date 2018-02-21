using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoormanAPI.Models;
using Microsoft.AspNetCore.SignalR;

namespace DoormanAPI.Hub
{
    public class DoormanHub : Microsoft.AspNetCore.SignalR.Hub
    {
	    public async Task Broadcast(string name, RoomOccupancySnapshotResultsVM roomOccupancySnapshotResults)
	    {
		    await this.Clients.All.InvokeAsync(name, roomOccupancySnapshotResults);
	    }
	}
}
