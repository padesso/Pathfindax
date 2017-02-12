﻿using System;

namespace Pathfindax.Nodes
{
	/// <summary>
	/// Represents a connection to another node
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public struct NodeConnection<TNode>
		where TNode : INode
	{
		/// <summary>
		/// The To where this connection is going to
		/// </summary>
		public TNode To;

		/// <summary>
		/// The collisions in this connection
		/// </summary>
		public PathfindaxCollisionCategory CollisionCategory;

		public NodeConnection(TNode To, PathfindaxCollisionCategory collisionCategory = PathfindaxCollisionCategory.None)
		{
			if(To == null) throw new ArgumentException("To cannot be null");
			this.To = To;
			CollisionCategory = collisionCategory;
		}

		public override string ToString()
		{
			return $"Connection to {To.Position}";
		}
	}
}
