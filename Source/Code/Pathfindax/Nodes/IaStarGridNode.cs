﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pathfindax.Nodes
{
	public class AstarGridNode: AstarGridNodeBase, IAStarGridNode 
	{		
		public IList<IAStarGridNode> Neighbours { get; set; }
		public AstarGridNode(IGridNodeBase source) : base(source)
		{
			Neighbours = new List<IAStarGridNode>();
		}
	}

	[Serializable]
	[DebuggerDisplay("{WorldPosition}")]
	public class AstarGridNodeBase : GridNodeBase
	{
		public int HCost { get; set; }
		public int GCost { get; set; }
		public int HeapIndex { get; set; }
		public int FCost => GCost + HCost;

		public AstarGridNodeBase(IGridNodeBase source) : base(source.WorldPosition, source.GridX, source.GridY, source.Walkable)
		{
			HCost = -1;
			GCost = -1;
		}

		public int CompareTo(IAStarGridNode other)
		{
			var compare = FCost.CompareTo(other.FCost);
			if (compare == 0)
			{
				compare = HCost.CompareTo(other.HCost);
			}
			return -compare;
		}

		public override string ToString()
		{
			return $"X:{GridX} Y:{GridY} Walkable: {Walkable}";
		}
	}
}