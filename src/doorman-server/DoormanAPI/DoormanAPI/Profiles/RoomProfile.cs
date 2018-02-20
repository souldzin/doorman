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
	    }
    }
}
