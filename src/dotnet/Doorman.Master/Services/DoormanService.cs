using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Doorman.Master.Entities;
using Doorman.Master.Hub;
using Doorman.Master.Models;
using Doorman.Master.Utility;
using Microsoft.AspNetCore.SignalR;

namespace Doorman.Master.Services
{
	public class DoormanService : IDoormanService
	{
		private readonly IHubContext<DoormanHub> _doormanHub;
		private readonly IDoormanContext _context;
		private readonly RandomTokenBuilder _tokenBuilder;

		public DoormanService(IDoormanContext context, IHubContext<DoormanHub> doormanHub)
		{
			_context = context;
			_doormanHub = doormanHub;
			_tokenBuilder = new RandomTokenBuilder();
		}

		IReadOnlyList<RoomVM> IDoormanService.GetRooms()
		{
			return Mapper.Map<IReadOnlyList<RoomVM>>(_context.Rooms.ToList());
		}

		PostRoomOccupancySnapshotResultsVM IDoormanService.SaveRoomOccupancySnapshot(int roomId, PostRoomOccupancySnapshotVM model)
		{
			var dbModel = Mapper.Map<PostRoomOccupancySnapshotVM, RoomOccupancySnapshot>(model);
			dbModel.RoomId = roomId;
			_context.RoomOccupancySnapshots.Add(dbModel);
			_context.SaveChanges();

			return new PostRoomOccupancySnapshotResultsVM();
		}

		PostRoomOccupancySnapshotResultsVM IDoormanService.GetRoomOccupancySnapshotById(int roomOccupancySnapshotId)
		{
			try
			{
				var dbModel =
					_context.RoomOccupancySnapshots.SingleOrDefault(x => x.RoomOccupancySnapshotId == roomOccupancySnapshotId);

				if (dbModel == null)
				{
					return null;
				}

				return Mapper.Map<RoomOccupancySnapshot, PostRoomOccupancySnapshotResultsVM>(dbModel);

			}
			catch (Exception ex)
			{
				return null;
			}
		}

		IReadOnlyList<PostRoomOccupancySnapshotResultsVM> IDoormanService.GetAll()
		{
			return Mapper.Map<IReadOnlyList<PostRoomOccupancySnapshotResultsVM>>(_context.RoomOccupancySnapshots.ToList());
		}

		PostRoomResultVM IDoormanService.RegisterRoom(string name, ClientConfigVM model)
		{
			var result = new PostRoomResultVM();

			try
			{
				var dbModel =
					_context.Rooms.SingleOrDefault(x => x.Description.Equals(name, StringComparison.OrdinalIgnoreCase));

				if (dbModel != null)
				{
					result = Mapper.Map<Room, PostRoomResultVM>(dbModel);

					var snapShotModels =
						_context.RoomOccupancySnapshots.Where(x => x.RoomId == dbModel.RoomId);

					if (snapShotModels.Any())
					{
						var snapShot = snapShotModels.OrderByDescending(x => x.CreateDateTime).First();
						Mapper.Map<RoomOccupancySnapshot, PostRoomResultVM>(snapShot, result);
					}
				}
				else
				{
					dbModel = new Room
					{
						Description = name,
						AccessToken = GetAccessToken()
					};

					_context.Rooms.Add(dbModel);

					_context.SaveChanges();

					result = Mapper.Map<Room, PostRoomResultVM>(dbModel);
				}

				return result;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		private string GetAccessToken()
		{
			return this._tokenBuilder.GenerateToken();
		}

		GetRoomResultVM IDoormanService.GetRoom(int roomId)
		{
			var result = new GetRoomResultVM();

			try
			{
				var dbModel =
					_context.Rooms.SingleOrDefault(x => x.RoomId == roomId);

				if (dbModel == null)
				{
					return null;
				}

				result = Mapper.Map<Room, GetRoomResultVM>(dbModel);

				var dbModels =
					_context.RoomOccupancySnapshots.Where(x => x.RoomId == dbModel.RoomId);

				if (!dbModels.Any())
					return result;
				{
					var snapShot = dbModels.OrderByDescending(x => x.CreateDateTime).First();
					Mapper.Map<RoomOccupancySnapshot, GetRoomResultVM>(snapShot, result);
				}

				return result;

			}
			catch (Exception ex)
			{
				return null;
			}
		}

		GetHistoricTrendVM IDoormanService.GetHistoricTrends(int roomId, DateTime start, DateTime end)
		{
			var result = new GetHistoricTrendVM();

			try
			{
				var dbModels =
					_context.RoomOccupancySnapshots.Where(x => x.RoomId == roomId).ToList();

				if (!dbModels.Any())
					return result;
				{

					var snapShot = dbModels.Where(x => x.CreateDateTime >= start && x.CreateDateTime <= end)
						.OrderByDescending(x => x.CreateDateTime).Take(30);

					result.Points = Mapper.Map<List<RoomOccupancySnapshot>, List<RoomOccupancySnapshotVM>>(snapShot.ToList());
				}

				return result;

			}
			catch (Exception ex)
			{
				return null;
			}
		}

		GetRecentTrendVM IDoormanService.GetRecentTrends(int roomId, int seconds)
		{
			var result = new GetRecentTrendVM();

			try
			{
				var dbModels =
					_context.RoomOccupancySnapshots.Where(x => x.RoomId == roomId).ToList();

				if (!dbModels.Any())
					return result;
				{
					var currentDateTime = DateTime.Now;
					var currentMinusSecondsDateTime = currentDateTime.AddSeconds(-seconds);
					var snapShot = dbModels.Where(x => x.CreateDateTime >= currentMinusSecondsDateTime && x.CreateDateTime <= currentDateTime)
						.OrderByDescending(x => x.CreateDateTime).Take(30);

					result.Points = Mapper.Map<List<RoomOccupancySnapshot>, List<RoomOccupancySnapshotVM>>(snapShot.ToList());
				}

				return result;

			}
			catch (Exception ex)
			{
				return null;
			}
		}

		GetStatVM IDoormanService.GetStats(int roomId, DateTime start, DateTime end)
		{
			var result = new GetStatVM();

			try
			{
				var dbModels =
					_context.RoomOccupancySnapshots.Where(x => x.RoomId == roomId).ToList();

				if (!dbModels.Any())
					return result;
				{

					var snapShot = dbModels.Where(x => x.CreateDateTime >= start && x.CreateDateTime <= end)
						.OrderByDescending(x => x.CreateDateTime).Take(30).ToList();

					if (!snapShot.Any())
						return result;
					{
						result.Average = Math.Round(snapShot.Average(x => x.Count), 1);
						result.Max = snapShot.Max(x => x.Count);
						result.MaxDate = snapShot.Max(x => x.CreateDateTime);

						var peakWeekday = snapShot.GroupBy(a => a.CreateDateTime.DayOfWeek).Select(g => g.OrderByDescending(x => x.Count).First()).ToList();
						var peakTime = snapShot.GroupBy(a => a.CreateDateTime.DayOfWeek).Select(g => g.OrderByDescending(x => x.CreateDateTime).First()).ToList();

						result.PeakWeekday = peakWeekday[0].CreateDateTime.DayOfWeek.ToString();
						result.PeakTime = peakTime[0].CreateDateTime.ToShortTimeString();

						result.StandardDeviation = CalculateStandardDeviation(snapShot.Select(x => x.Count).ToList());
					}
				}

				return result;

			}
			catch (Exception ex)
			{
				return null;
			}
		}

		void IDoormanService.SendBroadcast(int roomId)
		{
			var currentRoom = ((IDoormanService) this).GetRoom(roomId);
			var name = roomId.ToString();

			//Components for providing real-time bi-directional communication across the Web
			_doormanHub.Clients.All.InvokeAsync(name, currentRoom);
		}

		private double CalculateStandardDeviation(List<int> occupancyCountList)
		{
			var average = occupancyCountList.Average();
			var sumOfSquaresOfDifferences = occupancyCountList.Select(val => (val - average) * (val - average)).Sum();
			var result = Math.Sqrt(sumOfSquaresOfDifferences / occupancyCountList.Count);

			return Math.Round(result, 1);
		}
	}

	public interface IDoormanService
	{
		IReadOnlyList<RoomVM> GetRooms();
		PostRoomOccupancySnapshotResultsVM SaveRoomOccupancySnapshot(int roomId, PostRoomOccupancySnapshotVM model);
		IReadOnlyList<PostRoomOccupancySnapshotResultsVM> GetAll();
		PostRoomOccupancySnapshotResultsVM GetRoomOccupancySnapshotById(int roomOccupancySnapshotId);
		PostRoomResultVM RegisterRoom(string name, ClientConfigVM model);
		GetRoomResultVM GetRoom(int roomId);
		GetHistoricTrendVM GetHistoricTrends(int roomId, DateTime start, DateTime end);
		GetRecentTrendVM GetRecentTrends(int roomId, int seconds);
		GetStatVM GetStats(int roomId, DateTime start, DateTime end);
		void SendBroadcast(int roomId);
	}
}
