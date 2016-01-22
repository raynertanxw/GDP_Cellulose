using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathQuery 
{
	private static PathQuery s_Instance;

	private PointDatabase m_Database;

	private Dictionary<string,Edge> SearchFrontier;
	private Dictionary<string,Edge> ShortestPath;
	private Dictionary<string,float> fCost;
	private Dictionary<string,float> gCost;
	
	private string SourceIndex;
	private string TargetIndex;
	
	public PathQuery()
	{
		m_Database = PointDatabase.Instance;
	}
	
	public static PathQuery Instance
	{
		get
		{
			if(s_Instance == null)
			{
				s_Instance = new PathQuery();
			}
			return s_Instance;
		}
	}
	
	private float CalculateDistanceBetweenPoints(Point _P1, Point _P2)
	{
		return Mathf.Abs(Utility.Distance(_P1.Position,_P2.Position));
	}
	
	private void InitializeDictionaries()
	{
		List<string> keys = new List<string>(m_Database.Database.Keys);
		foreach(string key in keys)
		{
			SearchFrontier[key] = null;
			ShortestPath[key] = null;
			fCost[key] = 0f;
			gCost[key] = 0f;
		}
	}
	
	public void AStarSearch(Vector2 _Source, Vector2 _Target, bool _AllowUnwalkable)
	{
		SourceIndex = PointDatabase.Instance.GetIdealPoint(_Source,_Target).Index;
		TargetIndex = PointDatabase.Instance.GetClosestPointToPosition(_Target,_AllowUnwalkable).Index;
		
		List<Point> PointList = PointDatabase.Instance.ReturnDatabaseAsList();
		SearchFrontier = new Dictionary<string,Edge>();//store all points that are being searched
		ShortestPath = new Dictionary<string,Edge>();
		fCost = new Dictionary<string,float>();//cumulative cost to current node + heuristic cost from current node to target node
		gCost = new Dictionary<string,float>();//cumulative cost from the source node to the current node
		InitializeDictionaries();
	
		PriorityQueue priorityQueue = new PriorityQueue(fCost);
		priorityQueue.Add(SourceIndex);
		
		while(priorityQueue.Count() != 0)
		{
			string nextClosestPoint = priorityQueue.Dequeue();

			ShortestPath[nextClosestPoint] = SearchFrontier[nextClosestPoint];
			
			if(nextClosestPoint == TargetIndex)
			{
				return;
			}
			
			foreach(Edge edge in m_Database.Database[nextClosestPoint].Edges)
			{
				//heuristic cost from current node to target node
				float HCost = CalculateDistanceBetweenPoints(edge.End,m_Database.Database[TargetIndex]);
				//cumulative cost from the start node to the current node
				float GCost = gCost[nextClosestPoint] + edge.Cost;
				
				if(_AllowUnwalkable == false)
				{
					//Debug.Log("Only Walkable Points");
					if(SearchFrontier[edge.End.Index] == null && edge.End.Walkable == true)
					{
						fCost[edge.End.Index] = HCost + GCost;
						gCost[edge.End.Index] = GCost;
						priorityQueue.Add(edge.End.Index);
						SearchFrontier[edge.End.Index] = edge;
					}
					else if(GCost < gCost[edge.End.Index] && ShortestPath[edge.End.Index] == null && edge.End.Walkable == true)
					{
						fCost[edge.End.Index] = HCost + GCost;
						gCost[edge.End.Index] = GCost;
						priorityQueue.Add(edge.End.Index);
						SearchFrontier[edge.End.Index] = edge;
					}
				}
				else
				{
					//Debug.Log("Both Walkable and Unwalkable points");
					if(SearchFrontier[edge.End.Index] == null)
					{
						fCost[edge.End.Index] = HCost + GCost;
						gCost[edge.End.Index] = GCost;
						priorityQueue.Add(edge.End.Index);
						SearchFrontier[edge.End.Index] = edge;
					}
					else if(GCost < gCost[edge.End.Index] && ShortestPath[edge.End.Index] == null)
					{
						fCost[edge.End.Index] = HCost + GCost;
						gCost[edge.End.Index] = GCost;
						priorityQueue.Add(edge.End.Index);
						SearchFrontier[edge.End.Index] = edge;
					}
				}
			}
		}
		
	}
	
	private bool IsTwoPointsFreeToMove(Point _P1, Point _P2)
	{
		Vector2 direction = new Vector2(_P2.Position.x - _P1.Position.x, _P2.Position.y - _P1.Position.y);
		float distance = Utility.Distance(_P1.Position,_P2.Position);
		
		RaycastHit2D[] cast = Physics2D.RaycastAll(_P1.Position,direction,distance,Constants.s_onlyWallLayer);
		foreach(RaycastHit2D hit in cast)
		{
			if(hit.collider != null)
			{
				return false;
			}
		}
		return true;
	}
	
	private List<Point> RemoveExcessWallPoints(List<Point> _Path)
	{
		bool identifiedFirstWP = false;
		for(int i = 0; i < _Path.Count; i++)
		{
			if(i != 0 && (_Path[i].LIndex == "0" || _Path[i].LIndex == "8") && identifiedFirstWP == false)
			{
				identifiedFirstWP = true;
			}
			else if(i != 0 && (_Path[i].LIndex == "0" || _Path[i].LIndex == "8") && identifiedFirstWP == true)
			{
				_Path.Remove(_Path[i]);
			}
		}
		return _Path;
	}
	
	public List<Point> GetPathToTarget(Directness _directness)
	{
		List<Point> Path = new List<Point>();
		string currentKey = TargetIndex;
		Path.Insert(0,m_Database.Database[currentKey]);
		
		while(currentKey != SourceIndex && ShortestPath[currentKey] != null)
		{
			currentKey = ShortestPath[currentKey].Start.Index;
			Path.Insert(0,m_Database.Database[currentKey]);
		}

		int PossibleSmoothing = Mathf.CeilToInt(Path.Count/2);
		int smoothingAmount = PossibleSmoothing;
		int CalculateAmount = 0;
	
		//smooth 25%, 50%, 75% accordingly
		if(_directness == Directness.Low)
		{
			CalculateAmount = PossibleSmoothing/2;
		}
		else if(_directness == Directness.Mid)
		{
			CalculateAmount = PossibleSmoothing/4 * 3;
		}
		else if(_directness == Directness.High)
		{
			CalculateAmount = PossibleSmoothing;
		}
		
		smoothingAmount = (CalculateAmount > 0 && CalculateAmount < 1) ? 1 : Mathf.CeilToInt(PossibleSmoothing);
		
		int smoothDone = 0;
		
		for(int i = 0; i < Path.Count; i++)
		{
			if(i + 2 < Path.Count && IsTwoPointsFreeToMove(Path[i],Path[i + 2]))
			{
				Path.Remove(Path[i + 1]);
				smoothDone++;
				if(smoothDone >= smoothingAmount)
				{
					break;
				}
			}
		}
		
		Path = RemoveExcessWallPoints(Path);
		
		return Path;
	}
	
	public List<Point> RefinePathForTA(List<Point> _Path, Vector2 _TeleStart)
	{
		Point ClosestPointToStart = PointDatabase.Instance.GetClosestPointToPosition(_TeleStart,true);
		if(!IsPointInPath(ClosestPointToStart,_Path))
		{
			_Path.Add(ClosestPointToStart);
			return _Path;
		}
		return _Path;
	}
	
	private void DebugAllPathPoints(List<Point> _Path)
	{
		foreach(Point point in _Path)
		{
			Debug.Log("Point: " + point.Index);
		}
	}
	
	private bool IsPointInPath(Point _Point, List<Point> _Path)
	{
		foreach(Point point in _Path)
		{
			if(point == _Point)
			{
				return true;
			}
		}
		return false;
	}
	
	public Point ReturnVertSequenceStartPoint(List<Point> _Path)
	{
		Point ReferencePoint = _Path[_Path.Count - 1];
		Point StartPoint = _Path[_Path.Count - 1];
		for(int i = _Path.Count - 1; i >= 0; i--)
		{
			if(_Path[i].Position.x == ReferencePoint.Position.x)
			{
				StartPoint = _Path[i];
			}
			else
			{
				break;
			}
		}
		return StartPoint;
	}
	
	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
