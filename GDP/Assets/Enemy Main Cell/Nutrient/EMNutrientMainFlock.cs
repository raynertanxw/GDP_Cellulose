using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EMNutrientMainFlock : MonoBehaviour 
{
	// Weights for different behaviors
	public float FlockWeight = 1f;
	public float SeekWeight = .3f;
	
	EMNutrientMainAgent agent;

	public float fNeighbourRadius;
	public float fAlignmentWeight;
	public float fCohesionWeigth;
	public float fSeperationWeight;

	private Vector2 targetPosition;
	private Vector2 currentPosition;
	// List of neighbouring agents
	List<EMNutrientMainAgent> neighbouringAgents = new List<EMNutrientMainAgent>();
	
	void Start () 
	{
		// Initialization
		fNeighbourRadius = 1f;
		fAlignmentWeight = .5f;
		fCohesionWeigth = .5f;
		fSeperationWeight = .5f;
		// Add this behavior to the agent
		agent = GetComponent<EMNutrientMainAgent>();
		agent.AddBehaviour(this);
	}

	void Update ()
	{
		// Update enemy main cell's position
		targetPosition = EnemyMainFSM.Instance ().Position;
	}

	#region Behaviour
	// Return velocity based on neighbouring agents and weights
	public Vector2 GetVelocity()
	{
		currentPosition = (Vector2)transform.position;
		
		UpdateNeighbours();
		
		return Alignment() * fAlignmentWeight + Cohesion() * fCohesionWeigth + Seperation() * fSeperationWeight;
	}
	// Return velocity based on enemy main cell's position, current position and current velocity
	public Vector2 GetTargetVelocity()
	{
		return ((targetPosition - (Vector2)transform.position).normalized * agent.fMaxVelocity) - agent.currentVelocity;   
	}
	// Return velocity based on neighbouring agents' velocity
	private Vector2 Alignment()
	{
		Vector2 direction = Vector2.zero;
		
		if (neighbouringAgents.Count == 0)
			return direction;
		
		foreach (EMNutrientMainAgent agent in neighbouringAgents)
			direction += agent.currentVelocity;
		
		direction /= neighbouringAgents.Count;
		return direction.normalized;
	}
	// Return velocity based on neighbouring agents' position (towards)
	private Vector2 Cohesion()
	{
		Vector2 direction = Vector2.zero;
		
		foreach (EMNutrientMainAgent agent in neighbouringAgents)
			direction += (Vector2)agent.transform.position;
		
		direction /= neighbouringAgents.Count;
		
		return (direction - currentPosition).normalized;
	}
	// Return velocity based on neighbouring agents' position and current position(offwards)
	private Vector2 Seperation()
	{
		Vector2 direction = Vector2.zero;
		
		foreach (var agent in neighbouringAgents)
			direction += (Vector2)agent.transform.position - currentPosition;
		
		return (direction * -1);
	}
	
	void UpdateNeighbours()
	{
		// Clean up the list of neighbouring agents
		neighbouringAgents.Clear();
		// Add neighbouring agents into the list if they are within the radius
		foreach (EMNutrientMainAgent agent in EMNutrientMainAgent.AgentList)
		{
			if (Vector2.Distance((Vector2)agent.transform.position, currentPosition) < fNeighbourRadius)
			{
				neighbouringAgents.Add(agent);
			}
		}
	}
	#endregion
	// Remove behaviors from destroyed agent
	void OnDestroy()
	{
		if (agent != null)
			agent.RemoveBehaviour(this);
	}
}