using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Doorman.Master.Models
{
    public class GetTrendDataVM
    {
	    public GetTrendDataVM()
	    {
			Points = new List<TrendDataPointVM>();
		}

		[JsonProperty(PropertyName = "points")]
		public List<TrendDataPointVM> Points { get; set; }
    }
}
