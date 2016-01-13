using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECChargeCState : IECState {
	
	//A static float variable that state the maximum accelerateion that the enemy child cell can have at any point of time
	private static float fMaxAcceleration;
	
	//A boolean to state whether this enemy child cell had reach the final waypoint of the given path
	private bool bReachTarget;

	private GameObject TargetSource;

	private Vector2 TargetEndPos;

	private bool bReturnToIdle;
	
	//Constructor
	public ECChargeCState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		m_Child = _childCell;
		
		fMaxAcceleration = 10f;
		TargetEndPos = new Vector2(0f,0f);
	}
	
	
	public override void Enter()
	{
		//Set the charge target to be one of the player child cell
		m_ecFSM.Target = FindTargetChild();
		bReachTarget = false;
		bReturnToIdle = false;
		
		//If there is no target for the enemy child cell, return it back to idle state
		if(m_ecFSM.Target == null)
		{
			bReturnToIdle = true;
		}
		else if(m_ecFSM.Target != null)
		{
			m_Child.GetComponent<Rigidbody2D>().drag = 2.6f;
		}
	}
	
	public override void Execute()
	{
		//if at any point of time during attacking, it fall out of bound, transition the enemy child cell to dead state
		if(m_ecFSM.OutOfBound())
		{
			m_ecFSM.StartChildCorountine(m_ecFSM.PassThroughDeath(1f));
		}
	
		//If the target of this cell is dead, find another target if there is one, else just pass through and die/return back to main cell
		if(m_ecFSM.Target != null && (m_ecFSM.Target.name.Contains("Player_Child") && m_ecFSM.Target.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.Avoid || m_ecFSM.Target.name.Contains("Player_Child") && m_ecFSM.Target.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.Dead || m_ecFSM.Target.name.Contains("Squad_Child") && m_ecFSM.Target.GetComponent<SquadChildFSM>().EnumState == SCState.Dead))
		{
			GameObject NewTarget = FindTargetChild();
			if(NewTarget != null)
			{
				m_ecFSM.Target = NewTarget;
				return;
			}

			bReachTarget = true;
			TargetEndPos = new Vector2(m_Child.transform.position.x,-9.5f);
		}

		if(bReachTarget &&  m_ecFSM.HitBottomOfScreen())
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Dead,0.0f);
		}

	}
	
	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		//If the enemy child cell had not traveled through the path given and the target child cell is still alive, continue drive the enemy child cell towards the target player cell
		if(bReturnToIdle == false && bReachTarget == false && m_ecFSM.Target != null && !HasCellReachTargetPos(m_ecFSM.Target.transform.position))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_ecFSM.Target.transform.position,24f);
			Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours()) * 30f;
		}
		else if(bReturnToIdle == false && bReachTarget == true && !HasCellReachTargetPos(TargetEndPos))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,TargetEndPos,24f);
		}
		//If the enemy child cells is return back to the enemy main cell but has not reach the position, continue seek back to the main cell
		else if(bReturnToIdle && !HasCellReachTargetPos(m_Main.transform.position))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,15f);
		}
		//if the enemy child cell returned back to the enemy main cell, transition it back to the idle state
		else if(bReturnToIdle && HasCellReachTargetPos(m_Main.transform.position))
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0.0f);
		}

		//Clamp the acceleration of the enemy child cell to a maximum value and then add that acceleration force to the enemy child cell
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration);
		//Rotate the enemy child cell based on the direction of travel
		m_ecFSM.RotateToHeading();
	}
	
	public override void Exit()
	{
		//Stop the child cell from moving after it charges finish
		m_Child.GetComponent<Rigidbody2D>().drag = 0f;
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
	}
	
	//a function that check whether the target is still avaliable to be attacked and return a boolean to
	//represent the result
	private bool CheckIfTargetIsAvailable(GameObject _Target)
	{
		//Loop through all the enemy child cells and check whether another child cell had targeted the same
		//target. Then, return a boolean to represent the result
		//Debug.Log("original target: " + _Target);
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		foreach(EnemyChildFSM EC in ECList)
		{
			//Utility.CheckEmpty<GameObject>(EC.Target);
			//if(EC.Target != null){ Debug.Log("Compare: " + _Target.name + " " + EC.Target.name);}
			if(EC.name != m_Child.name && EC.CurrentStateEnum == ECState.ChargeChild && EC.Target != null && EC.Target.name == _Target.name)
			{
				return false;
			}
		}
		return true;
	}
	
	private GameObject FindTargetChild()
	{
		//Find the node to obtain a target child by evaluating which node is the most threatening
		if(TargetSource == null)
		{
			GameObject Source = GetMostThreateningSource();
			if(Source == null)
			{
				return null;
			}
			TargetSource = Source;
		}

		TargetEndPos = TargetSource.transform.position;

		//If the target to obtain a child cell to attack is any of the two nodes, loop through the cells within that specific node to get the closest child cell to the enemy child cell
		if(TargetSource.name.Contains("Node"))
		{
			List<PlayerChildFSM> m_PotentialTargets = TargetSource.GetComponent<Node_Manager>().GetNodeChildList();
			
			float fDistanceBetween = Mathf.Infinity;
			int nAvaliableEnemyChildCells = m_Main.GetComponent<EnemyMainFSM>().ECList.Count;
			GameObject m_TargetCell = null;
			
			for (int i = 0; i < m_PotentialTargets.Count; i++)
			{
				if (CheckIfTargetIsAvailable(m_PotentialTargets[i].gameObject) && (Vector2.Distance(m_Child.transform.position, m_PotentialTargets[i].transform.position)) < fDistanceBetween && m_PotentialTargets[i].GetCurrentState() != PCState.Dead && m_PotentialTargets[i].GetCurrentState() != PCState.Avoid)
				{
					fDistanceBetween = Vector2.Distance(m_Child.transform.position, m_PotentialTargets[i].transform.position);
					m_TargetCell = m_PotentialTargets[i].gameObject;
					break;
				}
			}
			if(m_TargetCell != null){return m_TargetCell;}
		}
		//Else If the target to obtain a child cell to attack is the squad captain cell, loop through the cells within that squad to get the closest child cell to the enemy child cell
		else if(TargetSource.name.Contains("Squad"))
		{
			List<SquadChildFSM> m_PotentialTargets = SquadChildFSM.GetAliveChildList();
			SquadChildFSM ClosestSquadChild = m_PotentialTargets[0];
			float ClosestDistance = Mathf.Infinity;
			
			foreach(SquadChildFSM SquadChild in m_PotentialTargets)
			{
				if(Vector2.Distance(m_Child.transform.position,SquadChild.transform.position) < ClosestDistance)
				{
					ClosestSquadChild = SquadChild;
					ClosestDistance = Vector2.Distance(m_Child.transform.position,SquadChild.transform.position);
				}
			}

			m_ecFSM.Target = ClosestSquadChild.gameObject;
			
			return m_ecFSM.Target;
		}
		
		//If there is no target for the child cell, transition it back to idle state
		return null;
	}
	
	private GameObject GetMostThreateningSource()
	{
		//Three different GameObject references to store the three potential source that produce a threat to the enemy main cell
		GameObject m_Squad = GameObject.Find("Squad_Captain_Cell");
		Node_Manager m_LeftNode = Node_Manager.GetNode(0);
		Node_Manager m_RightNode = Node_Manager.GetNode(1);
		
		//Calculate the scores for the three threat source on how threatening it is to the enemy main cell
		int nSquadScore = 0;
		if(m_Squad != null && m_Squad.transform.position.y != 1000f)
		{
			nSquadScore = m_Squad.GetComponent<PlayerSquadFSM>().AliveChildCount();
		}

		int nLeftScore = EvaluateNode(m_LeftNode);
		int nRightScore = EvaluateNode(m_RightNode);
		
		//Compare the score of the nodes and obtain the highest threat score, then return the most threatening source
		int nHighestThreat = Mathf.Max(Mathf.Max(nSquadScore,nLeftScore),nRightScore);
		
		if(nHighestThreat == 0)
		{
			return null;
		}
		
		if(nHighestThreat == nSquadScore)
		{
			return m_Squad;
		}
		else if(nHighestThreat == nLeftScore)
		{
			return m_LeftNode.gameObject;
		}
		else if(nHighestThreat == nRightScore)
		{
			return m_RightNode.gameObject;
		}
		
		return null;
	}
	
	//A function to evaluate the node based on the enemy main cell and player condition to return a score
	private int EvaluateNode (Node_Manager _Node)
	{
		//if the node contains no cell, it serve no threat to the enemy main cell
		if(_Node.GetNodeChildList().Count == 0)
		{
			return 0;
		}
		
		int nthreatLevel = 0;
		
		//increase score based on amount of cells in that node
		nthreatLevel += _Node.GetNodeChildList().Count;
		
		return nthreatLevel;
	}
	
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= 0.05f)
		{
			return true;
		}
		return false;
	}
	
	//A function that return a boolean on whether all the cells had reached the given position in the perimeter
	private bool HasAllCellReachTargetPos(Vector2 _Pos)
	{
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		foreach(EnemyChildFSM Child in ECList)
		{
			if(Child.CurrentStateEnum == ECState.Idle && !HasCellReachTargetPos(_Pos))
			{
				return false;
			}
		}
		return true;
	}
	
	//A function that return a boolean to show the target gameobject is dead 
	private bool IsTargetDead(GameObject _Target)
	{
		if(_Target.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.Dead)
		{
			return true;
		}
		return false;
	}
	
	//A function that return the first player child cell from a list of player child cells
	private GameObject GetFirstAvailableCell(List<PlayerChildFSM> _PotentialTarget)
	{
		for(int i = 0; i < _PotentialTarget.Count; i++)
		{
			if(_PotentialTarget[i] != null)
			{
				return _PotentialTarget[i].gameObject;
			}
		}
		return null;
	}
	
	//A function that return a list of GameObjects that are within a circular range to the enemy child cell
	private List<GameObject> TagNeighbours()
	{
		List<GameObject> Neighbours = new List<GameObject>();
		Collider2D[] Neighbouring = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2);
		
		for(int i = 0; i < Neighbouring.Length; i++)
		{
			if(Neighbouring[i].gameObject != m_Child && Neighbouring[i].gameObject.tag == Constants.s_strEnemyChildTag && Neighbouring[i].gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.ChargeChild)
			{
				Neighbours.Add(Neighbouring[i].gameObject);
			}
		}
		
		return Neighbours;
	}
}
