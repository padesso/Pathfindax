﻿using Pathfindax.Primitives;

namespace Pathfindax.PathfindEngine
{
	public interface IMultithreadedPathfinder : IPathfinder
	{
		/// <summary>
		/// This will process any completedpaths.
		/// </summary>
		void ProcessCompletedPaths();

		/// <summary>
		/// Starts the pathfinder.
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the pathfinder.
		/// </summary>
		void Stop();
	}

	public interface IPathfinder
	{
		/// <summary>
		/// Requests a path.
		/// </summary>
		/// <param name="pathRequest"></param>
		void RequestPath(PathRequest pathRequest);
		PositionF WorldSize { get; }
	}
}