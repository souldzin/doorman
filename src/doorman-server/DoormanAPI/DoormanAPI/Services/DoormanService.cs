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

		RoomOccupancySnapshotResultsVM IDoormanService.SaveRoomOccupancySnapshot(RoomOccupancySnapshotVM model)
		{
			try
			{
				var dbModel = Mapper.Map<RoomOccupancySnapshotVM, RoomOccupancySnapshot>(model);
				_context.RoomOccupancySnapshots.Add(dbModel);
				_context.SaveChanges();

				return Mapper.Map<RoomOccupancySnapshot, RoomOccupancySnapshotResultsVM>(dbModel);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		RoomOccupancySnapshotResultsVM IDoormanService.GetRoomOccupancySnapshotById(int roomOccupancySnapshotId)
		{
			try
			{
				var dbModel =
					_context.RoomOccupancySnapshots.SingleOrDefault(x => x.RoomOccupancySnapshotId == roomOccupancySnapshotId);

				if (dbModel == null)
				{
					return null;
				}

				return Mapper.Map<RoomOccupancySnapshot, RoomOccupancySnapshotResultsVM>(dbModel);

			}
			catch (Exception ex)
			{
				return null;
			}
		}

		IReadOnlyList<RoomOccupancySnapshotResultsVM> IDoormanService.GetAll()
		{
			return Mapper.Map<IReadOnlyList<RoomOccupancySnapshotResultsVM>>(_context.RoomOccupancySnapshots.ToList());
		}
	}

	public interface IDoormanService
	{
		IReadOnlyList<RoomVM> GetRooms();
		RoomOccupancySnapshotResultsVM SaveRoomOccupancySnapshot(RoomOccupancySnapshotVM model);
		IReadOnlyList<RoomOccupancySnapshotResultsVM> GetAll();
		RoomOccupancySnapshotResultsVM GetRoomOccupancySnapshotById(int roomOccupancySnapshotId);
	}
}
