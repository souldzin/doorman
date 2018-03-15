using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Doorman.Master.API.Entities
{
    public class DoormanContext : DbContext, IDoormanContext
	{
	    public DoormanContext(DbContextOptions<DoormanContext> options) : base(options)
	    {
			if(options.FindExtension<InMemoryOptionsExtension>() == null) {
				Database.Migrate();
			}
	    }

		public DbSet<Room> Rooms { get; set; }
		public DbSet<RoomOccupancySnapshot> RoomOccupancySnapshots { get; set; }
	}

	public interface IDoormanContext
	{
		int SaveChanges();
		DbSet<Room> Rooms { get; set; }
		DbSet<RoomOccupancySnapshot> RoomOccupancySnapshots { get; set; }
	}
}
