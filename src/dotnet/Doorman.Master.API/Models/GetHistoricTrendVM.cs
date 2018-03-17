using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Doorman.Master.API.Models
{
    public class GetHistoricTrendVM
    {
	    public GetHistoricTrendVM()
	    {
			Points = new List<RoomOccupancySnapshotVM>();
		}

		[JsonProperty(PropertyName = "points")]
		public List<RoomOccupancySnapshotVM> Points { get; set; }
    }
}
