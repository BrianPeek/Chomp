//
// From XNA sample here (Ms-PL):  http://xbox.create.msdn.com/en-US/education/catalog/sample/pathfinding
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonsterGame
{
	public enum SearchStatus
	{
		Stopped,
		Searching,
		NoPath,
		PathFound,
	}

	class PathFinder
	{
		/// <summary>
		/// Reresents one node in the search space
		/// </summary>
		private struct SearchNode
		{
			/// <summary>
			/// Location on the map
			/// </summary>
			public readonly Point Position;

			/// <summary>
			/// Distance to goal estimate
			/// </summary>
			public readonly int DistanceToGoal;
			
			/// <summary>
			/// Distance traveled from the start
			/// </summary>
			public readonly int DistanceTraveled;

			public SearchNode(Point mapPosition, int distanceToGoal, int distanceTraveled)
			{
				Position = mapPosition;
				DistanceToGoal = distanceToGoal;
				DistanceTraveled = distanceTraveled;
			}
		}

		// Holds search nodes that are avaliable to search
		private List<SearchNode> _openList;
		// Holds the nodes that have already been searched
		private List<SearchNode> _closedList;
		// Holds all the paths we've creted so far
		private Dictionary<Point, Point> _paths;
		// The map we're searching
		private Maze _maze;

		// Tells us if the search is stopped, started, finished or failed
		public SearchStatus SearchStatus { get; private set; }
	
		/// <summary>
		/// Toggles searching on and off
		/// </summary>
		public bool IsSearching
		{
			get { return SearchStatus == SearchStatus.Searching; }
			set 
			{
				if (SearchStatus == SearchStatus.Searching)
					SearchStatus = SearchStatus.Stopped;
				else if (SearchStatus == SearchStatus.Stopped)
					SearchStatus = SearchStatus.Searching;
			}
		}

		/// <summary>
		/// How many search steps have elapsed on this map
		/// </summary>
		public int TotalSearchSteps { get; private set; }
		public Point StartTile { get; set; }
		public Point EndTile { get; set; }

		public PathFinder()
		{
			TotalSearchSteps = 0;
		}

		/// <summary>
		/// Setup search
		/// </summary>
		/// <param name="mazeMap">Map to search</param>
		public void Initialize(Maze mazeMap)
		{
			SearchStatus = SearchStatus.Stopped;
			_openList = new List<SearchNode>();
			_closedList = new List<SearchNode>();
			_paths = new Dictionary<Point, Point>();
			_maze = mazeMap;
		}

		/// <summary>
		/// Search Update
		/// </summary>
		public void FindPath(Point start, Point end)
		{
			StartTile = start;
			EndTile = end;
			Reset();
			IsSearching	 = true;
			while(SearchStatus == SearchStatus.Searching)
				DoSearchStep();
		}

		/// <summary>
		/// Reset the search
		/// </summary>
		public void Reset()
		{
			SearchStatus = SearchStatus.Stopped;
			TotalSearchSteps = 0;
			_openList.Clear();
			_closedList.Clear();
			_paths.Clear();
			_openList.Add(new SearchNode(StartTile, StepDistance(StartTile, EndTile), 0));
		}

		private static int StepDistance(Point pointA, Point pointB)
		{
			int distanceX = Math.Abs(pointA.X - pointB.X);
			int distanceY = Math.Abs(pointA.Y - pointB.Y);

			return distanceX + distanceY;
		}

		private IEnumerable<Point> OpenMapTiles(Point mapLoc)
		{
			if (IsOpen(mapLoc.X, mapLoc.Y + 1))
				yield return new Point(mapLoc.X, mapLoc.Y + 1);
			if (IsOpen(mapLoc.X, mapLoc.Y - 1))
				yield return new Point(mapLoc.X, mapLoc.Y - 1);
			if (IsOpen(mapLoc.X + 1, mapLoc.Y))
				yield return new Point(mapLoc.X + 1, mapLoc.Y);
			if (IsOpen(mapLoc.X - 1, mapLoc.Y))
				yield return new Point(mapLoc.X - 1, mapLoc.Y);
		}

		private bool IsOpen(int col, int row)
		{
			return (row >= 0 && row < Maze.NumRows && col >= 0 && col < Maze.NumColumns) && 
					_maze.GetTile(row, col).TileType != MazeTile.MazeTileType.Wall;
		}

		/// <summary>
		/// This method find the next path node to visit, puts that node on the 
		/// closed list and adds any nodes adjacent to the visited node to the 
		/// open list.
		/// </summary>
		private void DoSearchStep()
		{
			SearchNode newOpenListNode;

			bool foundNewNode = SelectNodeToVisit(out newOpenListNode);
			if (foundNewNode)
			{
				Point currentPos = newOpenListNode.Position;
				foreach (Point point in OpenMapTiles(currentPos))
				{
					SearchNode mapTile = new SearchNode(point, StepDistance(point, EndTile), newOpenListNode.DistanceTraveled + 1);
					if (!InList(_openList,point) && !InList(_closedList,point))
					{
						_openList.Add(mapTile);
						_paths[point] = newOpenListNode.Position;
					}
				}
				if (currentPos == EndTile)
				{
					SearchStatus = SearchStatus.PathFound;
				}
				_openList.Remove(newOpenListNode);
				_closedList.Add(newOpenListNode);
			}
			else
			{
				SearchStatus = SearchStatus.NoPath;
			}
		}

		/// <summary>
		/// Determines if the given Point is inside the SearchNode list given
		/// </summary>
		private static bool InList(IEnumerable<SearchNode> list, Point point)
		{
			bool inList = false;
			foreach (SearchNode node in list)
			{
				if (node.Position == point)
				{
					inList = true;
				}
			}
			return inList;
		}

		/// <summary>
		/// This Method looks at everything in the open list and chooses the next 
		/// path to visit based on which search type is currently selected.
		/// </summary>
		/// <param name="result">The node to be visited</param>
		/// <returns>Whether or not SelectNodeToVisit found a node to examine
		/// </returns>
		private bool SelectNodeToVisit(out SearchNode result)
		{
			result = new SearchNode();
			bool success = false;
			float smallestDistance = float.PositiveInfinity;
			if (_openList.Count > 0)
			{
				TotalSearchSteps++;
				foreach (SearchNode node in _openList)
				{
					float currentDistance = node.DistanceToGoal;
					if(currentDistance < smallestDistance){
						success = true;
						result = node;
						smallestDistance = currentDistance;
					}
				}
			}
			return success;
		}

		/// <summary>
		/// Generates the path from start to end.
		/// </summary>
		/// <returns>The path from start to end</returns>
		public LinkedList<Point> FinalPath()
		{
			LinkedList<Point> path = new LinkedList<Point>();
			if (SearchStatus == SearchStatus.PathFound)
			{
				Point curPrev = EndTile;
				path.AddFirst(curPrev);
				while (_paths.ContainsKey(curPrev))
				{
					curPrev = _paths[curPrev];
					path.AddFirst(curPrev);
				}
			}
			return path;
		}
	}
}
