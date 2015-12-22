using UnityEngine;
using System.Collections;

public class Frontier_ExploredSet 
{
	// Use ArrayList for frontier
	private ArrayList nodes = new ArrayList();

	// Number of nodes in the frontier
	public int Length
	{
		get { return this.nodes.Count; }
	}

	// Check whether the node passed in is already in the frontier or not
	public bool Contains(object node)
	{
		return this.nodes.Contains(node);
	}

	// Get the first node in the frontier
	public EnemyNutrientNode First()
	{
		if (this.nodes.Count > 0)
		{
			return (EnemyNutrientNode)this.nodes[0];
		}
		return null;
	}

	// Add the node to the frontier and sort all nodes based on the estimated total cost
	public void Push(EnemyNutrientNode node)
	{
		this.nodes.Add(node);
		this.nodes.Sort();
	}

	// Remove the node from the frontier and sort the rest based on the estimated total cost
	public void Remove(EnemyNutrientNode node)
	{
		this.nodes.Remove(node);
		this.nodes.Sort();
	}	
}