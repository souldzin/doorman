﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Doorman.Master.Models
{
    public class PostRoomOccupancySnapshotResultsVM
    {
	    public PostRoomOccupancySnapshotResultsVM()
	    {
		    Success = true;
	    }
		[JsonProperty(PropertyName = "success")]
	    public bool Success { get; set; }
	}
}
