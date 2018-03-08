using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DoormanAPI.Entities;
using DoormanAPI.Models;

namespace DoormanAPI.Profiles
{
    public class RoomProfile : Profile
    {
	    public RoomProfile()
	    {
		    CreateMap<Room, RoomVM>();
		    CreateMap<Room, PostRoomResultVM>()
			    .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Description))
			    ;
		    CreateMap<Room, GetRoomResultVM>()
			    .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Description))
			    ;
		}
    }
}
