using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Doorman.Master.Models
{
    public class GetRecentTrendVM
	{
		public GetRecentTrendVM()
		{
			Points = new List<RoomOccupancyPointVM>();
		}

		[JsonProperty(PropertyName = "points")]
		public List<RoomOccupancyPointVM> Points { get; set; }
    }
}
