using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Doorman.Master.API.Models
{
    public class GetStatVM
	{
		[JsonProperty(PropertyName = "average")]
		public double Average { get; set; }

		[JsonProperty(PropertyName = "standardDeviation")]
		public double StandardDeviation { get; set; }

		[JsonProperty(PropertyName = "max")]
		public int Max { get; set; }
		[JsonProperty(PropertyName = "maxDate")]
		public DateTime MaxDate { get; set; }
		[JsonProperty(PropertyName = "peakWeekday")]
		public string PeakWeekday { get; set; }
		[JsonProperty(PropertyName = "peakTime")]
		public string PeakTime { get; set; }
	}
}
