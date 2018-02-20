using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DoormanAPI.Entities;
using DoormanAPI.Models;

namespace DoormanAPI.Profiles
{
    public class RoomOccupancySnapshotProfile : Profile
    {
	    public RoomOccupancySnapshotProfile()
	    {
		    CreateMap<RoomOccupancySnapshotVM, RoomOccupancySnapshot>()
			    .ForMember(dest => dest.CreateDateTime, opt => opt.UseValue(DateTime.Now));
		    CreateMap<RoomOccupancySnapshot, RoomOccupancySnapshotResultsVM>();
		}
	}
}
