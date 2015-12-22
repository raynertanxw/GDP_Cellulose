using UnityEngine;
using System.Collections;

public class EnemyNutrientAStar
{
	#region List fields
	
	public static Frontier_ExploredSet  closedList, openList;
	
	#endregion

	// Calculate the final path in the path finding
	private static ArrayList CalculatePath(EnemyNutrientNode node)
	{
		ArrayList list = new ArrayList();
		while (node != null)
		{
			list.Add(node);
			node = node.parent;
		}
		list.Reverse();
		return list;
	}

	// Calculate the estimated Heuristic cost to the goal
	private static float HeuristicEstimateCost(EnemyNutrientNode currentNode, EnemyNutrientNode goalNode)
	{
		Vector2 vecCost = (Vector2)currentNode.position - (Vector2)goalNode.position;
		return vecCost.magnitude;
	}

	// Find the path between start node and goal node using AStar Algorithm
	public static ArrayList FindPath(EnemyNutrientNode start, EnemyNutrientNode goal)
	{
		// Start Finding the path
		openList = new Frontier_ExploredSet ();
		openList.Push(start);
		start.fTotalCost = 0.0f;
		start.fEstimatedCost = HeuristicEstimateCost(start, goal);
		
		closedList = new Frontier_ExploredSet ();
		EnemyNutrientNode node = null;
		
		while (openList.Length != 0)
		{
			node = openList.First();
			
			if (node.position == goal.position)
			{
				return CalculatePath(node);
			}
			
			ArrayList neighbours = new ArrayList();
			MapManager.instance.GetNeighbours(node, neighbours);
			
			#region CheckNeighbours
			
			// Get the Neighbours
			for (int i = 0; i < neighbours.Count; i++)
			{
				// Cost between neighbour nodes
				EnemyNutrientNode neighbourNode = (EnemyNutrientNode)neighbours[i];
				
				if (!closedList.Contains(neighbourNode))
				{					
					// Cost from current node to this neighbour node
					float cost = HeuristicEstimateCost(node, neighbourNode);	
					
					// Total Cost So Far from start to this neighbour node
					float totalCost = node.fTotalCost + cost;
					
					// Estimated cost for neighbour node to the goal
					float neighbourNodeEstCost = HeuristicEstimateCost(neighbourNode, goal);					
					
					// Assign neighbour node properties
					neighbourNode.fTotalCost = totalCost;
					neighbourNode.parent = node;
					neighbourNode.fEstimatedCost = totalCost + neighbourNodeEstCost;
					
					// Add the neighbour node to the list if not already existed in the list
					if (!openList.Contains(neighbourNode))
					{
						openList.Push(neighbourNode);
					}
				}
			}
			
			#endregion
			
			closedList.Push(node);
			openList.Remove(node);
		}
		
		// If finished looping and cannot find the goal then return null
		if (node.position != goal.position)
		{
			//Debug.LogError("Goal Not Found");
			//Debug.Log ("Last node position:" + node.position);
			//Debug.Log ("Goal node position:" + goal.position);
			return null;
		}
		
		// Calculate the path based on the final node
		return CalculatePath(node);
	}
}