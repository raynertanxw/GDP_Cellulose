using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EMNutrientMainFlock : MonoBehaviour 
{
	public float FlockWeight = 1f;
	public float SeekWeight = .2f;
	
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
	
	void OnDestroy()
	{
		if (agent != null)
			agent.RemoveBehaviour(this);
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
		Vector2 averagePosition = Vector2.zero;
		
		foreach (EMNutrientMainAgent agent in neighbouringAgents)
			averagePosition += (Vector2)agent.transform.position;
		
		averagePosition /= neighbouringAgents.Count;
		
		return (averagePosition - currentPosition).normalized;
	}
	
	private Vector2 Seperation()
	{
		Vector2 moveDirection = Vector2.zero;
		
		foreach (var agent in neighbouringAgents)
			moveDirection += (Vector2)agent.transform.position - currentPosition;
		
		return (moveDirection * -1);
	}
	
	void UpdateNeighbours()
	{
		neighbouringAgents.Clear();
		
		foreach (EMNutrientMainAgent agent in EMNutrientMainAgent.AgentList)
		{
			if (Vector3.Distance(agent.transform.position, currentPosition) < fNeighbourRadius)
				neighbouringAgents.Add(agent);
		}
	}
	#endregion
}