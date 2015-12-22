using UnityEngine;
using System.Collections;
using System;

public class EnemyNutrientNode : IComparable
{
	#region Fields
	public float fEstimatedCost;        // Estimated cost from the current node to the goal node
	public float fTotalCost;			// Total cost for the current node
	public bool bAccessible;            // Whether the current node is accessible or not
	public EnemyNutrientNode parent;                 // Parent node of the current node in the linked list
	public Vector2 position;            // Position of the current node
	#endregion

	// Default Constructor
	public EnemyNutrientNode()
	{
		this.fEstimatedCost = 0.0f;
		this.fTotalCost = 1.0f;
		this.bAccessible = false;
		this.parent = null;
	}

	// Constructor
	public EnemyNutrientNode(Vector2 pos)
	{
		this.fEstimatedCost = 0.0f;
		this.fTotalCost = 1.0f;
		this.bAccessible = true;
		this.parent = null;
		
		this.position = pos;
	}

	// Mark the current node as unaccessible
	public void MarkAsUnaccessible()
	{
		this.bAccessible = false;
	}

	// Override the class CompareTo function for sorting
	// Applied when calling Sort.Sort from ArrayList
	// Compare using the estimated total cost between two nodes
	public int CompareTo(object obj)
	{
		EnemyNutrientNode node = (EnemyNutrientNode)obj;
		if (this.fEstimatedCost < node.fEstimatedCost)
			return -1;
		if (this.fEstimatedCost > node.fEstimatedCost)
			return 1;
		
		return 0;
	}
}
