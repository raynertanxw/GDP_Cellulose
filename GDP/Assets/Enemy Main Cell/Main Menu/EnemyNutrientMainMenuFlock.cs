using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyNutrientMainMenuFlock : MonoBehaviour {

	// Weights for different behaviors
	[SerializeField]
	private float fFlockWeight;
	public float FlockWeight { get { return fFlockWeight; } }
	[SerializeField]
	private float fSeekWeight;
	public float SeekWeight { get { return fSeekWeight; } }
	
	EnemyNutrientMainMenuAgent agent;
	
	public float fNeighbourRadius;
	public float fAlignmentWeight;
	public float fCohesionWeigth;
	public float fSeperationWeight;
	
	private GameObject target;
	private Vector2 currentPosition;

	private float fFlowFactor;
	// List of neighbouring agents
	List<EnemyNutrientMainMenuAgent> neighbouringAgents = new List<EnemyNutrientMainMenuAgent>();
	
	void Start () 
	{
		// Get the target object
		int index = Random.Range (1, 6);
		string targetName = "Enemy_Cell_" + index.ToString ();
		target = GameObject.Find (targetName);
		// Initialization
		fFlockWeight = 1f;
		// Number behind makes sure the seek weight is so small that the nutrient never mve away from the main cell
		fSeekWeight = .175f;
		fNeighbourRadius = 1f;
		fAlignmentWeight = .6f;
		fCohesionWeigth = .6f;
		fSeperationWeight = .8f;
		// Factor that simulates flowing
		fFlowFactor = 0.5f;
		// Add this behavior to the agent
		agent = GetComponent<EnemyNutrientMainMenuAgent>();
		agent.AddBehaviour(this);
	}
	
	void Update ()
	{
		
	}
	
	#region Behaviour
	// Return velocity based on neighbouring agents and weights
	public Vector2 GetVelocity()
	{
		currentPosition = (Vector2)transform.position;
		
		UpdateNeighbours();
		
		return Alignment() * fAlignmentWeight + Cohesion() * fCohesionWeigth + Seperation() * fSeperationWeight - new Vector2 (fFlowFactor, 0f);
	}
	// Return velocity based on enemy main cell's position, current position and current velocity
	public Vector2 GetTargetVelocity()
	{
		if (target != null)
			return (((Vector2)target.transform.position - (Vector2)transform.position).normalized * agent.fMaxVelocity) - agent.currentVelocity;
		else
			return Vector2.zero;
	}
	// Return velocity based on neighbouring agents' velocity
	private Vector2 Alignment()
	{
		Vector2 direction = Vector2.zero;
		
		if (neighbouringAgents.Count == 0)
			return direction;
		
		foreach (EnemyNutrientMainMenuAgent agent in neighbouringAgents)
			direction += agent.currentVelocity;
		
		direction /= neighbouringAgents.Count;
		return direction.normalized;
	}
	// Return velocity based on neighbouring agents' position (towards)
	private Vector2 Cohesion()
	{
		Vector2 direction = Vector2.zero;
		
		foreach (EnemyNutrientMainMenuAgent agent in neighbouringAgents)
			direction += (Vector2)agent.transform.position;
		
		direction /= neighbouringAgents.Count;
		
		return (direction - currentPosition).normalized;
	}
	// Return velocity based on neighbouring agents' position and current position(offwards)
	private Vector2 Seperation()
	{
		Vector2 direction = Vector2.zero;
		
		foreach (var agent in neighbouringAgents) 
		{
			if (Vector2.Distance ((Vector2)agent.transform.position, currentPosition) != 0f)
				direction += ((Vector2)agent.transform.position - currentPosition).normalized / 
					Vector2.Distance ((Vector2)agent.transform.position, currentPosition);
		}
		
		direction /= neighbouringAgents.Count;
		
		return (direction * -1);
	}
	
	void UpdateNeighbours()
	{
		// Clean up the list of neighbouring agents
		neighbouringAgents.Clear();
		// Add neighbouring agents into the list if they are within the radius
		if (EnemyNutrientMainMenuAgent.AgentList != null) 
		{
			foreach (EnemyNutrientMainMenuAgent agent in EnemyNutrientMainMenuAgent.AgentList)
			{
				if (Vector2.Distance ((Vector2)agent.transform.position, currentPosition) < fNeighbourRadius)
				{
					neighbouringAgents.Add (agent);
				}
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
