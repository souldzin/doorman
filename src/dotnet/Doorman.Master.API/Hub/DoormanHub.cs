using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doorman.Master.API.Models;
using Microsoft.AspNetCore.SignalR;

namespace Doorman.Master.API.Hub
{
    public class DoormanHub : Microsoft.AspNetCore.SignalR.Hub
    {
	    public async Task Broadcast(string name, GetRoomResultVM roomResult)
	    {
		    await this.Clients.All.InvokeAsync(name, roomResult);
	    }
	}
}
