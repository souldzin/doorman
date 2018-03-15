using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Doorman.Master.API.Entities;
using Doorman.Master.API.Models;

namespace Doorman.Master.API.Profiles
{
    public class RoomOccupancySnapshotProfile : Profile
    {
	    public RoomOccupancySnapshotProfile()
	    {
		    CreateMap<PostRoomOccupancySnapshotVM, RoomOccupancySnapshot>()
			    .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.OccupancyCount))
			    .ForMember(dest => dest.CreateDateTime, opt => opt.ResolveUsing(src => DateTime.Now));

			CreateMap<RoomOccupancySnapshot, PostRoomResultVM>()
			    .ForMember(dest => dest.RoomId, opt => opt.Ignore())
				.ForMember(dest => dest.OccupancyCount, opt => opt.MapFrom(src => src.Count))
				;

		    CreateMap<RoomOccupancySnapshot, GetRoomResultVM>()
			    .ForMember(dest => dest.RoomId, opt => opt.Ignore())
			    .ForMember(dest => dest.RoomName, opt => opt.Ignore())
			    .ForMember(dest => dest.OccupancyCount, opt => opt.MapFrom(src => src.Count))
				;

		    CreateMap<RoomOccupancySnapshot, RoomOccupancySnapshotVM>()
			    .ForMember(dest => dest.OccupancyCount, opt => opt.MapFrom(src => src.Count))
				;
		}
	}
}
