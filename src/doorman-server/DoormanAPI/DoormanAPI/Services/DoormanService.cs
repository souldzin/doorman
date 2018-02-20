using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DoormanAPI.Entities;
using DoormanAPI.Models;

namespace DoormanAPI.Services
{
	public class DoormanService : IDoormanService
	{
		private readonly IDoormanContext _context;

		public DoormanService(IDoormanContext context)
		{
			_context = context;
		}

		IReadOnlyList<RoomVM> IDoormanService.GetRooms()
		{
			return Mapper.Map<IReadOnlyList<RoomVM>>(_context.Rooms.ToList());
		}

		int? IDoormanService.SaveRoomOccupancySnapshot(RoomOccupancySnapshotVM model)
		{
			try
			{
				var dbModel = Mapper.Map<RoomOccupancySnapshotVM, RoomOccupancySnapshot>(model);
				_context.RoomOccupancySnapshots.Add(dbModel);
				_context.SaveChanges();

				return dbModel.RoomOccupancySnapshotId;
			}
			catch (Exception e)
			{
				return null;
			}
		}
	}

	public interface IDoormanService
	{
		IReadOnlyList<RoomVM> GetRooms();
		int? SaveRoomOccupancySnapshot(RoomOccupancySnapshotVM model);
	}
}
