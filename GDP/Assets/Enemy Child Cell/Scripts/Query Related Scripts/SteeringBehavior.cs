using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class SteeringBehavior
{
	//Return a steering force towards the target position
	public static Vector2 Seek(GameObject _Agent, Vector2 _TargetPos, float _Speed)
	{
		Vector2 Difference = new Vector2(_Agent.transform.position.x - _TargetPos.x,_Agent.transform.position.y - _TargetPos.y);
		return -Difference.normalized * _Speed;	
	}
	
	//Return a steering force away from the target position
	public static Vector2 Flee(GameObject _Agent, Vector2 _TargetPos, float _Speed)
	{
		Vector2 TargetVelocity = new Vector2 (_TargetPos.x - _Agent.transform.position.x, _TargetPos.y - _Agent.transform.position.y).normalized * _Speed;
		return TargetVelocity - _Agent.GetComponent<Rigidbody2D>().velocity;
	}
	
	public static Vector2 Arrive(GameObject _Agent, Vector2 _TargetPos, float _Deceleration)
	{
		float DistanceToTarget = Utility.Distance(_Agent.transform.position, _TargetPos);
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
		Vector2 Difference = Vector2.zero;
		Vector2 AgentPos = _Agent.transform.position;
		Vector2 GOPos = Vector2.zero;
		float Distance = 0f;
		
		for(int i = 0; i < _SameGOType.Count; i++)
		{
			if(_SameGOType[i] != _Agent)
			{
				GOPos = _SameGOType[i].transform.position;
				Difference = GOPos - AgentPos;
				Distance = Difference.magnitude;
				
				if(Distance <= 1f)
				{
					Steering -= Difference;
				} 
			}
		}

		Vector2 SeperationNormal = Steering.normalized;
		float SeperationMagnitude =  Steering.magnitude;
		float MinimumMagnitude = 0.6f;
		Steering = Mathf.Clamp(SeperationMagnitude,MinimumMagnitude,SeperationMagnitude) * SeperationNormal;

		return Steering;
	}
	
	public static Vector2 Cohesion(GameObject _Agent, List<GameObject> Neighbours, float _Speed)
	{
		Vector2 Steering = Vector2.zero;
		Vector2 CenterPoint = Vector2.zero;
		Vector2 NeighbourPos = Vector2.zero;
		int NeighbourCount = 0;
		
		for(int i = 0; i < Neighbours.Count; i++)
		{
			NeighbourPos = Neighbours[i].transform.position;
			CenterPoint += NeighbourPos;
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
		Vector2 Difference = _CrowdCenter - _CurrentTarget.Position;
		return -Difference.normalized * _CrowdSpeed;
	} 
	
	public static Vector2 ShakeOnSpot(GameObject _Agent, float _ShakeDistance, float _ShakeStrength)
	{
		Vector2 RandomPoint = Random.insideUnitCircle * _ShakeDistance;
		Vector2 AgentPos = _Agent.transform.position;
		return Seek(_Agent,AgentPos + RandomPoint,_ShakeStrength);
	}
}
