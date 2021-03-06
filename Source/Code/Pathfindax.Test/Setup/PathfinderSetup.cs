﻿using Duality;
using NSubstitute;
using Pathfindax.Algorithms;
using Pathfindax.Factories;
using Pathfindax.Graph;
using Pathfindax.Nodes;
using Pathfindax.PathfindEngine;
using Pathfindax.Paths;

namespace Pathfindax.Test.Setup
{
	public static class PathfinderSetup
	{
		public static Pathfinder<IDefinitionNodeNetwork, IPathfindNodeNetwork, IPath> Create(int threads, bool succesTrue = true)
		{
			var manager = new PathfindaxManager();
			return manager.CreatePathfinder(Substitute.For<IDefinitionNodeNetwork>(), Substitute.For<IPathFindAlgorithm<IPathfindNodeNetwork, IPath>>(), (definitionNodeNetwork, algorithm) =>
			{
				var nodeGrid = Substitute.For<IPathfindNodeNetwork>();
				algorithm.FindPath(null, null, out var _).ReturnsForAnyArgs(x =>
				{
					x[2] = succesTrue;
					return new NodePath(new[] { new DefinitionNode(0, Vector2.Zero) }, new Transformer(new Vector2(1, 1)));
				});
				return PathfinderFactory.CreateRequestProcesser(nodeGrid, algorithm);
			}, threads);
		}
	}
}