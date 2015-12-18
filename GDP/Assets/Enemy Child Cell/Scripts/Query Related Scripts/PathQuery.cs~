using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathQuery 
{
	private PointDatabase m_Database;
	private PositionQuery m_PositionQuery;
	
	private GameObject PlayerMain;
	private GameObject EnemyMain;
	
	private Point[] SearchFrontier;
	private Point[] ShortestPath;
	private float[] fCost;
	private float[] gCost;
	
	private string SourceIndex;
	private string TargetIndex;
	
	public PathQuery()
	{
		m_Database = PointDatabase.Instance;
		m_PositionQuery = PositionQuery.Instance;
		PlayerMain = GameObject.Find("Player_Cell");
		EnemyMain = GameObject.Find("Enemy_Cell");
	}
	
	public void AStarSearch(string _Source, string _Target, Directness _Directness)
	{
		Point source = m_Database.Database[_Source];
		Point target = m_Database.Database[_Target];
		
		List<Point> PointList = PointDatabase.Instance.ReturnDatabaseAsList();
		SearchFrontier = new Point[PointList.Count];//store all points that are being searched
		ShortestPath = new Point[PointList.Count];
		fCost = new float[PointList.Count];//cumulative cost to current node + heuristic cost from current node to target node
		gCost = new float[PointList.Count];//cumulative cost from the source node to the current node
	}
	
	/*public Point[] GetPathToTarget()
	{
	
	}*/
}
