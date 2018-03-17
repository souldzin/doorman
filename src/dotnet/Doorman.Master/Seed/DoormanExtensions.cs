using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doorman.Master.Entities;

namespace Doorman.Master.Seed
{
    public static class DoormanExtensions
    {
	    public static void EnsureSeedDataForContext(this DoormanContext context)
	    {
		    if (!context.Rooms.Any())
		    {
			    var rooms = new List<Room>
			    {
				    new Room {Description = "Computer Science Hall"},
				    new Room {Description = "Software Engineering Hall"}
			    };

			    context.Rooms.AddRange(rooms);
		    }

		    context.SaveChanges();
	    }
    }
}
