using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EMNutrientMainFlock : MonoBehaviour 
{
	public float FlockWeight = 1f;
	public float SeekWeight = .3f;
	
	EMNutrientMainAgent agent;

	public float fNeighbourRadius = 1f;
	public float fAlignmentWeight = .7f;
	public float fCohesionWeigth = .5f;
	public float fSeperationWeight = .2f;

	private Vector2 targetPosition;
	
	List<EMNutrientMainAgent> neighbouringAgents = new List<EMNutrientMainAgent>();
	Vector2 currentPosition;
	
	void Start () 
	{
		agent = GetComponent<EMNutrientMainAgent>();
		agent.AddBehaviour(this);
	}

	void Update ()
	{
		targetPosition = EnemyMainFSM.Instance ().Position;
	}

	#region Behaviour
	public Vector2 GetVelocity()
	{
		currentPosition = (Vector2)transform.position;
		
		UpdateNeighbours();
		
		return Alignment() * fAlignmentWeight + Cohesion() * fCohesionWeigth + Seperation() * fSeperationWeight;
	}

	public Vector2 GetTargetVelocity()
	{
		return ((targetPosition - (Vector2)transform.position).normalized * agent.fMaxVelocity) - agent.currentVelocity;   
	}
	
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
	
	private Vector2 Cohesion()
	{
		Vector2 direction = Vector2.zero;
		
		foreach (EMNutrientMainAgent agent in neighbouringAgents)
			direction += (Vector2)agent.transform.position;
		
		direction /= neighbouringAgents.Count;
		
		return (direction - currentPosition).normalized;
	}
	
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
		
		foreach (EMNutrientMainAgent agent in EMNutrientMainAgent.AgentList)
		{
			if (Vector3.Distance(agent.transform.position, currentPosition) < fNeighbourRadius)
				neighbouringAgents.Add(agent);
		}
	}
	#endregion

	void OnDestroy()
	{
		if (agent != null)
			agent.RemoveBehaviour(this);
	}
}