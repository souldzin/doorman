using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DoormanAPI.Models
{
    public class GetRecentTrendVM
	{
		public GetRecentTrendVM()
		{
			Points = new List<RoomOccupancySnapshotVM>();
		}

		[JsonProperty(PropertyName = "points")]
		public List<RoomOccupancySnapshotVM> Points { get; set; }
    }
}
