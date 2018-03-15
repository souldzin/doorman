using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DoormanAPI.Models
{
    public class ClientConfigVM
    {
		[JsonProperty(PropertyName = "client_id")]
	    public string ClientId { get; set; }

	    [JsonProperty(PropertyName = "client_secret")]
	    public string ClientSecret { get; set; }
	}
}
