using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DoormanAPI.Entities
{
    public class DoormanContext : DbContext, IDoormanContext
	{
	    public DoormanContext(DbContextOptions<DoormanContext> options) : base(options)
	    {
			Database.Migrate();
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
