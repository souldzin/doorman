using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Doorman.Master.Models
{
    public class PostRoomOccupancySnapshotVM
    {
		[JsonProperty(PropertyName = "roomID")]
	    public int RoomId { get; set; }
	    [JsonProperty(PropertyName = "occupancyCount")]
		public int OccupancyCount { get; set; }
	}
}
