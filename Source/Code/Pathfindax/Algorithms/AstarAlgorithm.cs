﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Pathfindax.Collections;
using Pathfindax.Grid;
using Pathfindax.Primitives;

namespace Pathfindax.Algorithms
{
	/// <summary>
	/// Class that implements the A* algorithm to find paths
	/// </summary>
	public class AStarAlgorithm : IPathFindAlgorithm<IAStarGridNode>
	{
		/// <inheritdoc />
		public IList<IGridNode> FindPath(INodeGrid<IAStarGridNode> nodeGrid, PositionF pathStart, PositionF pathEnd)
		{
			var startNode = nodeGrid.GetNode(pathStart);
			var endNode = nodeGrid.GetNode(pathEnd);
			return FindPath(nodeGrid, startNode, endNode);
		}

		private IList<IGridNode> FindPath(INodeGrid<IAStarGridNode> nodeGrid, IAStarGridNode startGridNode, IAStarGridNode targetGridNode)
		{
			try
			{
				var sw = new Stopwatch();
				sw.Start();

				var pathSucces = false;
				if (startGridNode == targetGridNode)
				{
					return new List<IGridNode> { targetGridNode };
				}
				if (startGridNode.Walkable && targetGridNode.Walkable)
				{
					var openSet = new MinHeap<IAStarGridNode>(nodeGrid.NodeCount);
					var closedSet = new HashSet<IAStarGridNode>();
					var itterations = 0;
					var neighbourUpdates = 0;
					openSet.Add(startGridNode);
					while (openSet.Count > 0)
					{
						itterations++;
						var currentNode = openSet.RemoveFirst();
						closedSet.Add(currentNode);

						if (currentNode == targetGridNode)
						{
							sw.Stop();
							Debug.WriteLine($"Path found in {sw.ElapsedMilliseconds} ms. Itterations: {itterations} Neighbourupdates: {neighbourUpdates}");
							pathSucces = true;
							break;
						}

						foreach (var neighbour in nodeGrid.GetNeighbours(currentNode))
						{
							if (!neighbour.Walkable || closedSet.Contains(neighbour))
							{
								continue;
							}

							var newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
							if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
							{
								neighbour.GCost = newMovementCostToNeighbour;
								neighbour.HCost = GetDistance(neighbour, targetGridNode);
								neighbour.Parent = currentNode;
								neighbourUpdates++;
								if (!openSet.Contains(neighbour))
									openSet.Add(neighbour);
							}
						}
					}
				}
				if (pathSucces)
				{
					return RetracePath(startGridNode, targetGridNode);
				}
				Debug.WriteLine("Did not find a path :(");
				return null;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				return null;
			}
		}

		private IList<IGridNode> RetracePath(IAStarGridNode startGridNode, IAStarGridNode endGridNode)
		{
			var path = new List<IGridNode>();
			var currentNode = endGridNode;

			while (true)
			{
				path.Add(currentNode);
				if (currentNode == startGridNode) break;
				currentNode = currentNode.Parent;
			}
			path.Reverse();
			return path;
		}

		private static int GetDistance(IGridNode gridNodeA, IGridNode gridNodeB)
		{
			var dstX = Math.Abs(gridNodeA.GridX - gridNodeB.GridX);
			var dstY = Math.Abs(gridNodeA.GridY - gridNodeB.GridY);
			return dstY + dstX;
		}
	}
}