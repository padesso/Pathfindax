﻿using Duality;
using Pathfindax.Nodes;

namespace Pathfindax.Graph
{
	public class GridClearanceGenerator : IPathfindNodeGenerator<AstarNode>
	{
		private readonly int _maxClearance;
		private readonly IDefinitionNodeGrid _definitionNodeGrid;

		public GridClearanceGenerator(IDefinitionNodeGrid definitionNodeGrid, int maxClearance)
		{
			_maxClearance = maxClearance;
			_definitionNodeGrid = definitionNodeGrid;
		}

		public void Generate(AstarNode[] pathfindingNetwork, PathfindaxCollisionCategory collisionCategory)
		{
			if (_maxClearance < 0)
			{
				for (var i = 0; i < pathfindingNetwork.Length; i++)
				{
					pathfindingNetwork[i].Clearance = int.MaxValue;
				}
			}
			else
			{
				for (int y = 0; y < _definitionNodeGrid.DefinitionNodeArray.Height; y++)
				{
					for (int x = 0; x < _definitionNodeGrid.DefinitionNodeArray.Width; x++)
					{
						var definitionNode = _definitionNodeGrid.DefinitionNodeArray[x, y];
						var clearance = CalculateGridNodeClearances(definitionNode, collisionCategory, _maxClearance);
						pathfindingNetwork[definitionNode.Index.Index].Clearance = clearance;
					}
				}
			}
		}

		/// <summary>
		/// Calculates the clearances up to a maximum <paramref name="maxClearance"/>
		/// </summary>
		/// <param name="definitionNode"></param>
		/// <param name="collisionCategory"></param>
		/// <param name="maxClearance"></param>
		/// <returns></returns>
		public float CalculateGridNodeClearances(DefinitionNode definitionNode, PathfindaxCollisionCategory collisionCategory, int maxClearance)
		{
			var fromCoordinates = new Point2((int) definitionNode.Position.X, (int) definitionNode.Position.Y);
			for (var checkClearance = 0; checkClearance < maxClearance; checkClearance++)
			{
				var nextClearanceIsBlocked = false;
				for (var x = 0; x < checkClearance + 1; x++)
				{
					switch (IsBlocked(x + fromCoordinates.X, checkClearance + fromCoordinates.Y, collisionCategory, fromCoordinates))
					{
						case BlockType.Current:
							return checkClearance;
						case BlockType.Next:
							nextClearanceIsBlocked = true;
							break;
					}
				}

				for (var y = 0; y < checkClearance; y++)
				{
					switch (IsBlocked(checkClearance + fromCoordinates.X, y + fromCoordinates.Y, collisionCategory, fromCoordinates))
					{
						case BlockType.Current:
							return checkClearance;
						case BlockType.Next:
							nextClearanceIsBlocked = true;
							break;
					}
				}

				if (nextClearanceIsBlocked)
				{
					var isBlocked = true;
					for (var i = 0; i < definitionNode.Connections.Count; i++)
					{
						if ((definitionNode.Connections[i].CollisionCategory & collisionCategory) == 0) isBlocked = false;
					}

					return isBlocked ? checkClearance : checkClearance + 1;
				}
			}
			return maxClearance;
		}

		private BlockType IsBlocked(int x, int y, PathfindaxCollisionCategory collisionCategory, Point2 fromCoordinates)
		{
			if (x >= _definitionNodeGrid.DefinitionNodeArray.Width || y >= _definitionNodeGrid.DefinitionNodeArray.Height)
				return BlockType.Current;
			var definitionNode = _definitionNodeGrid.DefinitionNodeArray[x, y];
			var blockType = BlockType.None;
			foreach (var nodeConnection in definitionNode.Connections)
			{
				if ((nodeConnection.CollisionCategory & collisionCategory) != 0)
				{
					var toNode = _definitionNodeGrid.DefinitionNodeArray[nodeConnection.To.Index];
					var toCoordinates = new Point2((int) toNode.Position.X, (int) toNode.Position.Y);
					if (toCoordinates.X >= fromCoordinates.X && toCoordinates.Y >= fromCoordinates.Y)
					{
						if (toCoordinates.X >= x || toCoordinates.Y >= y)
						{
							blockType = BlockType.Next;
						}
						else
						{
							return BlockType.Current;
						}
					}
				}
			}
			return blockType;
		}

		public enum BlockType
		{
			None,
			Next,
			Current,
		}
	}
}