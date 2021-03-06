﻿using System;
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
				{
					return result;
				}

				var snapShot = dbModels.OrderByDescending(x => x.CreateDateTime).First();
				Mapper.Map<RoomOccupancySnapshot, GetRoomResultVM>(snapShot, result);

				return result;

			}
			catch (Exception ex)
			{
				return null;
			}
		}

		GetTrendDataVM IDoormanService.GetHistoricTrends(int roomId, DateTime start, DateTime end)
		{
			return GetTrendData(roomId, start, end);
		}

		private DateTime GetEarliestTrendDate(int roomId, DateTime startAt) {
			var earliest = _context.RoomOccupancySnapshots
				.Where(x => x.RoomId == roomId)
				.Where(x => x.CreateDateTime <= startAt)
				.Select(x => (DateTime?)x.CreateDateTime)
				.Max();

			if(!earliest.HasValue) {
				return startAt;
			} else {
				return earliest.Value;
			}
		}

		private List<DateTime> GetDateTimesBetween(DateTime startAt, DateTime endAt) {
			var maxPoints = 30;
			var interval = (endAt.Ticks - startAt.Ticks)/maxPoints;
			var times = new List<DateTime>(maxPoints);

			for(int i = 1; i < maxPoints; i++) {
				times.Add(startAt.AddTicks(i * interval));
			}

			var rounded = times
				.Select(x => new DateTime(
					x.Year,
					x.Month,
					x.Day,
					x.Hour,
					x.Minute,
					0
				))
				.Distinct();

			return new [] { startAt }
				.Concat(rounded)
				.Concat(new [] { endAt })
				.ToList();
		}

		private List<TrendDataPointVM> BuildSummaryList(IEnumerable<DateTime> times, IEnumerable<TrendDataPointVM> snapshots) {
			var points = times
				.OrderBy(x => x)
				.Select(x => new TrendDataPointVM {
					OccupancyCount = 0,
					Timestamp = x
				})
				.ToList();

			// Console.WriteLine("POINTS");
			// Console.WriteLine(string.Join(", ", points.Select(x => x.Timestamp.ToString() + ": " + x.OccupancyCount)));
			// Console.WriteLine("SNAPSHOTS");
			// Console.WriteLine(string.Join(", ", snapshots.Select(x => x.Timestamp.ToString() + ": " + x.OccupancyCount)));

			var currentPoints = points.Skip(0);

			foreach(var snapshot in snapshots) {
				foreach(var point in currentPoints) {
					if(point.Timestamp >= snapshot.Timestamp) {
						point.OccupancyCount = snapshot.OccupancyCount;
					}
				}
			}

			return points;
		}

		GetTrendDataVM IDoormanService.GetRecentTrends(int roomId, int seconds)
		{
			var endAt = DateTime.Now;
			var startAt = endAt.AddSeconds(-seconds);

			return GetTrendData(roomId, startAt, endAt);
		}

		private GetTrendDataVM GetTrendData(int roomId, DateTime startAt, DateTime endAt) 
		{
			var startFilterAt = GetEarliestTrendDate(roomId, startAt);

			var mostRecentAtFound = _context.RoomOccupancySnapshots
				.Where(x => x.RoomId == roomId)
				.Select(x => (DateTime?)x.CreateDateTime)
				.Max();

			// If no snapshots exist, just return empty.
			if(!mostRecentAtFound.HasValue) {
				return new GetTrendDataVM();
			}

			var mostRecentAt = mostRecentAtFound.Value;

			var mostRecentSnapshot = _context.RoomOccupancySnapshots
				.Where(x => x.RoomId == roomId)
				.Where(x => x.CreateDateTime >= mostRecentAt)
				.Select(x => new TrendDataPointVM {
					Timestamp = x.CreateDateTime,
					OccupancyCount = x.Count
				})
				.FirstOrDefault();

			var snapshots = _context.RoomOccupancySnapshots
				.Where(x => x.RoomId == roomId)
				.Where(x => x.CreateDateTime >= startFilterAt)
				.Where(x => x.CreateDateTime <= endAt)
				.GroupBy(x => new DateTime(
					x.CreateDateTime.Year, 
					x.CreateDateTime.Month,
					x.CreateDateTime.Day,
					x.CreateDateTime.Hour,
					x.CreateDateTime.Minute,
					0
				))
				.Select(g => new TrendDataPointVM {
					Timestamp = g.Key,
					OccupancyCount = g.Max(x => x.Count)
				}) 
				.OrderBy(x => x.Timestamp)
				.ToList()
				.Concat(new [] { mostRecentSnapshot });

			var times = GetDateTimesBetween(startAt, endAt);
			var result = BuildSummaryList(times, snapshots);

			return new GetTrendDataVM() {
				Points = result
			};
		}

		GetStatVM IDoormanService.GetStats(int roomId, DateTime start, DateTime end)
		{
			var points = GetTrendData(roomId, start, end).Points;
			var result = new GetStatVM();

			if (!points.Any()) 
			{
				return result;
			}

			result.Average = Math.Round(points.Average(x => x.OccupancyCount), 1);
			result.Max = points.Max(x => x.OccupancyCount);
			result.MaxDate = points.LastOrDefault(x => x.OccupancyCount == result.Max).Timestamp;

			result.PeakWeekday = result.MaxDate.DayOfWeek.ToString();
			result.PeakTime = result.MaxDate.ToShortTimeString();

			result.StandardDeviation = CalculateStandardDeviation(points.Select(x => x.OccupancyCount).ToList());

			return result;
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
		GetTrendDataVM GetHistoricTrends(int roomId, DateTime start, DateTime end);
		GetTrendDataVM GetRecentTrends(int roomId, int seconds);
		GetStatVM GetStats(int roomId, DateTime start, DateTime end);
		void SendBroadcast(int roomId);
	}
}
