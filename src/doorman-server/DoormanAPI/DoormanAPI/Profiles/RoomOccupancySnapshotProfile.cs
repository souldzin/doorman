using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DoormanAPI.Models;

namespace DoormanAPI.Profiles
{
    public class RoomOccupancySnapshotProfile : Profile
    {
	    public RoomOccupancySnapshotProfile()
	    {
		    CreateMap<RoomOccupancySnapshotVM, RoomOccupancySnapshotProfile>();
		    CreateMap<RoomOccupancySnapshotProfile, RoomOccupancySnapshotResultsVM>();
		}
	}
}
