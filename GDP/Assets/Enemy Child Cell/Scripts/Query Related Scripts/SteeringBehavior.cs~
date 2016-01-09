using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class SteeringBehavior
{
	//Return a steering force towards the target position
	public static Vector2 Seek(GameObject _Agent, Vector2 _TargetPos, float _Speed)
	{
		Vector2 AgentPos = _Agent.transform.position;
		Vector2 Difference = new Vector2(AgentPos.x - _TargetPos.x,AgentPos.y - _TargetPos.y);
		Vector2 Direction = -Difference.normalized;
	    return Direction * _Speed;	
	}
	
	//Return a steering force away from the target position
	public static Vector2 Flee(GameObject _Agent, Vector2 _TargetPos, float _Speed)
	{
		Vector2 TargetVelocity = new Vector2 (_TargetPos.x - _Agent.transform.position.x, _TargetPos.y - _Agent.transform.position.y).normalized * _Speed;
		return TargetVelocity - _Agent.GetComponent<Rigidbody2D>().velocity;
	}
	
	public static Vector2 Arrive(GameObject _Agent, Vector2 _TargetPos, float _Deceleration)
	{
		float DistanceToTarget = Vector2.Distance(_Agent.transform.position, _TargetPos);
		if(DistanceToTarget > 0)
		{
			float Speed = DistanceToTarget/_Deceleration;
			Vector2 DirectionToTarget = new Vector2(_TargetPos.x - _Agent.transform.position.x, _TargetPos.y - _Agent.transform.position.y);
			Vector2 TargetVelocity = DirectionToTarget * Speed/DistanceToTarget;
			return (TargetVelocity - _Agent.GetComponent<Rigidbody2D>().velocity);
		}
		return new Vector2(0f,0f);
	}
	
	public static Vector2 Evade(GameObject _Agent, GameObject _Hunter, float _Speed)
	{
		Vector2 DirectionToHunter = _Hunter.transform.position - _Agent.transform.position;
		float LookAhead = DirectionToHunter.magnitude /(2 * _Hunter.GetComponent<Rigidbody2D>().velocity.magnitude);
		
		Vector2 PredictedVelocity = new Vector2(_Hunter.GetComponent<Rigidbody2D>().velocity.x * LookAhead, _Hunter.GetComponent<Rigidbody2D>().velocity.y * LookAhead);
		return -Flee(_Agent,new Vector2(_Hunter.transform.position.x + PredictedVelocity.x, _Hunter.transform.position.y + PredictedVelocity.y),_Speed);
	}
	
	public static Vector2 Seperation(GameObject _Agent, List<GameObject> _SameGOType)
	{
		//Conrad Parker's setup
		Vector2 Steering = new Vector2(0f,0f);
		foreach(GameObject GO in _SameGOType)
		{
			if(GO != _Agent)
			{
				Vector2 difference = new Vector2(GO.transform.position.x - _Agent.transform.position.x, GO.transform.position.y - _Agent.transform.position.y);
				float distance = difference.magnitude;
				if(distance <= 1f)
				{
					Steering.x -= difference.x;
					Steering.y -= difference.y;
				} 
			}
		}

		Vector2 SeperationNormal = Steering.normalized;
		float SeperationMagnitude =  Steering.magnitude;
		float MinimumMagnitude = 0.6f;
		Steering = Mathf.Clamp(SeperationMagnitude,MinimumMagnitude,SeperationMagnitude) * SeperationNormal;

		//if(Steering.magnitude < 0.4f) {Debug.Log(_Agent.name + ": " + Steering.magnitude);}

		return Steering;
	}
	
	public static Vector2 SeperationByPos(GameObject _Agent, List<Vector2> _Positions)
	{
		Vector2 Steering = new Vector2(0f,0f);
		foreach(Vector2 Position in _Positions)
		{
			if(Position.x != _Agent.transform.position.x && Position.y != _Agent.transform.position.y)
			{
				Vector2 difference = new Vector2(Position.x - _Agent.transform.position.x, Position.y - _Agent.transform.position.y);
				float distance = difference.magnitude;
				if(distance <= 1f)
				{
					Steering.x -= difference.x;
					Steering.y -= difference.y;
				} 
			}
		}
		
		return Steering;
	}
	
	public static Vector2 Cohesion(GameObject _Agent, List<GameObject> Neighbours, float _Speed)
	{
		Vector2 Steering = new Vector2(0f,0f);
		Vector2 CenterPoint = new Vector2(0f,0f);
		
		int NeighbourCount = 0;
		
		for(int i = 0; i < Neighbours.Count; i++)
		{
			CenterPoint.x += Neighbours[i].transform.position.x;
			CenterPoint.y += Neighbours[i].transform.position.y;
			NeighbourCount++;
		}
		
		if(NeighbourCount > 0)
		{
			CenterPoint /= NeighbourCount;
			Steering = Seek(_Agent,CenterPoint,_Speed);
		}

		return Steering;
	}
	
	public static Vector2 CrowdAlignment(Vector2 _CrowdCenter, Point _CurrentTarget, float _CrowdSpeed)
	{
		Vector2 Difference = new Vector2(_CrowdCenter.x - _CurrentTarget.Position.x,_CrowdCenter.y - _CurrentTarget.Position.y);
		Vector2 Direction = -Difference.normalized;
		return Direction * _CrowdSpeed;
	} 
	
	public static Vector2 GatherAllECSameState(GameObject _Agent, ECState _State, float _Speed)
	{
		List<GameObject> EnemyChilds = GameObject.FindGameObjectsWithTag(Constants.s_strEnemyChildTag).ToList();
		List<GameObject> Filtered = new List<GameObject>();
		foreach(GameObject Child in EnemyChilds)
		{
			if(Child.GetComponent<EnemyChildFSM>().CurrentStateEnum == _State)
			{
				Filtered.Add(Child);
			}
		}
		return Cohesion(_Agent,Filtered,_Speed);
	}
	
	public static Vector2 ShakeOnSpot(GameObject _Agent, float _ShakeDistance, float _ShakeStrength)
	{
		Vector2 RandomPoint = Random.insideUnitCircle * _ShakeDistance;
		Vector2 RandomPosition = new Vector2(_Agent.transform.position.x + RandomPoint.x, _Agent.transform.position.y + RandomPoint.y);
		return Seek(_Agent,RandomPosition,_ShakeStrength);
	}
	
	public static Vector2 EnsureZeroOverlap(GameObject _Agent, List<GameObject> Neighbours)
	{
		float OverlapLength = _Agent.GetComponent<SpriteRenderer>().bounds.size.x/4;
		float SteerMagnitude = 10f;
		int OverlapCount = 0;
		Vector2 Steering = Vector2.zero;
		
		foreach(GameObject Neighbour in Neighbours)
		{
			if(Vector2.Distance(_Agent.transform.position, Neighbour.transform.position) < OverlapLength)
			{
				//OverlapCount++;
				Steering.x += Neighbour.transform.position.x - _Agent.transform.position.x;
				Steering.y += Neighbour.transform.position.y - _Agent.transform.position.y;
				
			}
		}
		
		//Steering /= OverlapCount;
		Steering = Steering.normalized;
		Steering *= SteerMagnitude;
		//Debug.Log("Agent: " +_Agent.name + " " + Steering);
		return new Vector2(Steering.y,Steering.x);
	}
}
