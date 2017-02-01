﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Pathfindax.Algorithms;
using Pathfindax.Grid;
using Pathfindax.Nodes;
using Pathfindax.PathfindEngine;
using Pathfindax.Primitives;

namespace Pathfindax.Test
{
	[TestFixture]
	public class MultithreadedPathfinderTests
	{
		private MultithreadedPathfinder<INodeGrid<IAStarGridNode>, IAStarGridNode> SetupMultithreadedPathfinder(int threads)
		{
			var algorithm = Substitute.For<IPathFindAlgorithm<INodeGrid<IAStarGridNode>, IAStarGridNode>>();
			var nodeGrid = Substitute.For<INodeGrid<IAStarGridNode>>();
			return new MultithreadedPathfinder<INodeGrid<IAStarGridNode>, IAStarGridNode>(new List<INodeGrid<IAStarGridNode>> { nodeGrid }, algorithm, threads);
		}

		[Test]
		public void RequestPath_SingleThread_NoExceptions()
		{
			var multithreadedPathfinder = SetupMultithreadedPathfinder(1);

			multithreadedPathfinder.Start();
			var start = new PositionF(0.5f, 0.5f);
			var end = new PositionF(127.5f, 127.5f);
			var taskCompletionSource = new TaskCompletionSource<bool>();
			Action<CompletedPath> success = pathrequest =>
			{
				taskCompletionSource.SetResult(true);
			};
			multithreadedPathfinder.RequestPath(new PathRequest(success, start, end, 1));
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			while (!taskCompletionSource.Task.IsCompleted)
			{
				Thread.Sleep(10);
				multithreadedPathfinder.ProcessCompletedPaths();
				if (stopWatch.ElapsedMilliseconds > 2000) throw new TimeoutException();
			}
			Assert.AreEqual(taskCompletionSource.Task.Result, true);
		}

		[Test]
		public void RequestPath_MultipleThreads_NoExceptions()
		{
			var multithreadedPathfinder = SetupMultithreadedPathfinder(4);
			multithreadedPathfinder.Start();
			var start = new PositionF(0.5f, 0.5f);
			var end = new PositionF(127.5f, 127.5f);

			var pathRequests = new PathRequest[100];
			var taskCompletionSources = new TaskCompletionSource<bool>[pathRequests.Length];
			var success = new Action<CompletedPath>[pathRequests.Length];
			for (var i = 0; i < pathRequests.Length; i++)
			{
				var i1 = i;
				taskCompletionSources[i1] = new TaskCompletionSource<bool>();
				success[i] = request => taskCompletionSources[i1].SetResult(true);
				multithreadedPathfinder.RequestPath(new PathRequest(success[i], start, end, 1));
			}
			var tasks = taskCompletionSources.Select(x => x.Task).ToList();
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			while (!tasks.All(x => x.IsCompleted))
			{
				Thread.Sleep(10);
				multithreadedPathfinder.ProcessCompletedPaths();
				if (stopWatch.ElapsedMilliseconds > 2000) throw new TimeoutException();
			}
			foreach (var taskCompletionSource in taskCompletionSources)
			{
				Assert.AreEqual(taskCompletionSource.Task.Result, true);
			}
		}
	}
}
