using UnityEngine;
using System.Collections;

// Using A* algorithm instead of other informed (heuristic) search strategies such as memory-bounded heuristic search
// The reason is by using algorithms such as iterative-deepening A* (IDA*), recursive best-first search (RBFS) and memory-bounded A* (MA*),
// they certainly help to save memory space, by a lot actually for some of them, but at a fairly big cost in exection time.
// Since time complexity is out biggest problem rather than space complexity, A* is optimally efficient for any given consistent heuristic and thus is 
// the best solution to out pathfinding problem here
public class EnemyNutrientAStar
{
	#region List fields
	
	public static Frontier_ExploredSet  exploredSet, frontier;
	
	#endregion

	// Get the real path by reversing the path found
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

	// Calculate the estimated Heuristic cost to the goal node
	private static float HeuristicEstimateCost(EnemyNutrientNode currentNode, EnemyNutrientNode goalNode)
	{
		Vector2 vecCost = currentNode.position - goalNode.position;
		return vecCost.magnitude;
	}

	// Find the path between start node and goal node using AStar Algorithm
	public static ArrayList FindPath(EnemyNutrientNode start, EnemyNutrientNode goal)
	{
		// Start Finding the path
		frontier = new Frontier_ExploredSet ();						// Initialize the frontier
		exploredSet = new Frontier_ExploredSet ();					// Initialize the explored set
		frontier.Push(start);										// Add the start node to the frontier
		start.fTotalCost = 0.0f;									// Initialize the total cost for the start node
		start.fEstimatedCost = HeuristicEstimateCost(start, goal);	// Calculate the heuristic

		EnemyNutrientNode node = null;								// Initialize a node to use

		// While the frontier isnot empty
		while (frontier.Length != 0)
		{
			node = frontier.First();								// Take the first node in the frontier

			// If the node is the goal node, then the path is found
			// Instead of checking after adding the node to the explored set, we check it when it's first generated
			// The purpose is to save time from looking for a more optimal path
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
				
				if (!exploredSet.Contains(neighbourNode))
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
					
					// Add the neighbour node to the frontier if not already existed in the frontier
					if (!frontier.Contains(neighbourNode))
					{
						frontier.Push(neighbourNode);
					}
				}
			}
			
			#endregion
			
			exploredSet.Push(node);									// Add the node to the explored set as it is expanded
			frontier.Remove(node);									// Remove the node from the frontier
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