using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoormanAPI.Models
{
    public class RoomOccupancySnapshotResultsVM
    {
	    public int RoomOccupancySnapshotId { get; set; }
	    public int RoomId { get; set; }
	    public int Count { get; set; }
	    public DateTime CreateDateTime { get; set; }
	}
}
