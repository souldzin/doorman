using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Doorman.Master.Models
{
    public class TrendDataPointVM
    {
	    [JsonProperty(PropertyName = "occupancyCount")]
	    public int OccupancyCount { get; set; }
	    [JsonProperty(PropertyName = "timestamp")]
	    public DateTime Timestamp { get; set; }
	}
}