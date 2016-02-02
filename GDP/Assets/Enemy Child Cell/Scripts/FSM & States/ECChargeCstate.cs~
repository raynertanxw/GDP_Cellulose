using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECChargeCState : IECState {

	private static float m_fMaxAcceleration;

	private bool m_bReachTarget;
	private bool m_bReturnToIdle;
	private bool m_bSqueezeToggle;
	private bool m_bSqueezeDone;
	
	private GameObject m_TargetSource;
	private Vector2 m_TargetEndPos;
	private static Vector3 m_ShrinkRate;
	
	private enum Style {Aggressive,Defensive};

	//Constructor
	public ECChargeCState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		m_Child = _childCell;

		m_fMaxAcceleration = 18f;
		m_TargetEndPos = Vector2.zero;
		m_ShrinkRate = new Vector3(-0.4f, 0.4f, 0.0f);
	}


	public override void Enter()
	{
		if(m_ecFSM.m_AttackTarget != null){CheckIfEnoughCells();}
		
		//Set the charge target to be one of the player child cell
		m_ecFSM.Target = FindTargetChild();
		m_bReachTarget = false;
		m_bReturnToIdle = false;

		//If there is no target for the enemy child cell, return it back to idle state
		if(m_ecFSM.Target == null)
		{
			m_bReturnToIdle = true;
		}
		else if(m_ecFSM.Target != null)
		{
			m_ecFSM.rigidbody2D.drag = 2.6f;
		}
		
		ECTracker.s_Instance.ChargeChildCells.Add(m_ecFSM);
	}

	public override void Execute()
	{
		//if at any point of time during attacking, it fall out of bound, transition the enemy child cell to dead state
		if(m_bSqueezeDone && m_ecFSM.OutOfBound())
		{
			m_ecFSM.StartChildCorountine(m_ecFSM.PassThroughDeath(1f));
		}
		
		if(m_bSqueezeDone && m_ecFSM.HitBottomOfScreen())
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Dead,0.0f);
		}
		
		if(!m_bSqueezeDone)
		{
			if(!m_bSqueezeToggle && m_ecFSM.Target != null)
			{
				m_ecFSM.StartChildCorountine(SqueezeBeforeCharge(m_ecFSM.Target));
				m_bSqueezeToggle = true;
			}
		}
		
		//If the target of this cell is dead, find another target if there is one, else just pass through and die/return back to main cell
		if(m_bSqueezeDone && m_ecFSM.Target != null && (m_ecFSM.Target.name.Contains("Player_Child") && m_ecFSM.Target.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.Avoid || m_ecFSM.Target.name.Contains("Player_Child") && m_ecFSM.Target.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.Dead || m_ecFSM.Target.name.Contains("Squad_Child") && m_ecFSM.Target.GetComponent<SquadChildFSM>().EnumState == SCState.Dead))
		{
			GameObject NewTarget = FindTargetChild();
			if(NewTarget != null)
			{
				m_ecFSM.Target = NewTarget;
				return;
			}

			m_bReachTarget = true;
			m_TargetEndPos = new Vector2(m_Child.transform.position.x,-9.5f);
		}

		if(m_bReachTarget &&  m_ecFSM.HitBottomOfScreen())
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Dead,0.0f);
		}

	}

	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		//If the enemy child cell had not traveled through the path given and the target child cell is still alive, continue drive the enemy child cell towards the target player cell
		if(m_bSqueezeDone && !m_bReturnToIdle && !m_bReachTarget && m_ecFSM.Target != null && !HasCellReachTargetPos(m_ecFSM.Target.transform.position))
		{
			Acceleration += SteeringBehavior.Pursuit(m_Child,m_ecFSM.Target,24f);
			Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours()) * 30f;
			if(m_Child.transform.localScale.y < 1f && m_Child.transform.localScale.x > 0.5f){m_Child.transform.localScale += m_ShrinkRate;}
		}
		else if(m_bSqueezeDone && !m_bReturnToIdle && m_bReachTarget == true && !HasCellReachTargetPos(m_TargetEndPos))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_TargetEndPos,24f);
		}
		//If the enemy child cells is return back to the enemy main cell but has not reach the position, continue seek back to the main cell
		else if(m_bReturnToIdle && !HasCellReachTargetPos(m_Main.transform.position))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,15f);
		}
		//if the enemy child cell returned back to the enemy main cell, transition it back to the idle state
		else if(m_bReturnToIdle && HasCellReachTargetPos(m_Main.transform.position))
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0.0f);
		}

		//Clamp the acceleration of the enemy child cell to a maximum value and then add that acceleration force to the enemy child cell
		Acceleration = Vector2.ClampMagnitude(Acceleration,m_fMaxAcceleration);
		m_ecFSM.rigidbody2D.AddForce(Acceleration);
		//Rotate the enemy child cell based on the direction of travel
		if(m_bSqueezeDone) {m_ecFSM.RotateToHeading();}
	}

	public override void Exit()
	{
		//Stop the child cell from moving after it charges finish
		m_ecFSM.rigidbody2D.drag = 0f;
		m_ecFSM.rigidbody2D.velocity = Vector2.zero;
		m_Child.transform.localScale = Vector3.one;
		ECTracker.s_Instance.ChargeChildCells.Remove(m_ecFSM);
	}

	//a function that check whether the target is still avaliable to be attacked and return a boolean to
	//represent the result
	private bool CheckIfTargetIsAvailable(GameObject _Target)
	{
		//Loop through all the enemy child cells and check whether another child cell had targeted the same
		//target. Then, return a boolean to represent the result
		
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		for(int i = 0; i < ECList.Count; i++)
		{
			if(ECList[i].name != m_Child.name && ECList[i].CurrentStateEnum == ECState.ChargeChild && ECList[i].Target != null && ECList[i].Target.name == _Target.name)
			{
				return false;
			}
		}
		return true;
	}

	private GameObject FindTargetChild()
	{
		//Find the node to obtain a target child by evaluating which node is the most threatening
		if(m_TargetSource == null)
		{
			GameObject Source = m_ecFSM.m_AttackTarget;
			if(Source == null)
			{
				return null;
			}
			m_TargetSource = Source;
		}

		m_TargetEndPos = m_TargetSource.transform.position;

		//If the target to obtain a child cell to attack is any of the two nodes, loop through the cells within that specific node to get the closest child cell to the enemy child cell
		if(m_TargetSource.name.Contains("Node"))
		{
			List<PlayerChildFSM> m_PotentialTargets = new List<PlayerChildFSM>();
			if(m_TargetSource.name.Contains("Left"))
			{
				for(int i = 0; i < PlayerChildFSM.childrenInLeftNode.Length - 1; i++)
				{
					if(PlayerChildFSM.childrenInLeftNode[i] == -1){break;}
					
					m_PotentialTargets.Add(PlayerChildFSM.playerChildPool[PlayerChildFSM.childrenInLeftNode[i]]);
				}
			}
			else if(m_TargetSource.name.Contains("Right"))
			{
				for(int i = 0; i < PlayerChildFSM.childrenInRightNode.Length - 1; i++)
				{
					if(PlayerChildFSM.childrenInRightNode[i] == -1){break;}
				
					m_PotentialTargets.Add(PlayerChildFSM.playerChildPool[PlayerChildFSM.childrenInRightNode[i]]);
				}
			}

			float fDistanceBetween = Mathf.Infinity;
			GameObject m_TargetCell = null;
			float ChildToPotentialTarget = 0f;
			PCState TargetCurrentState = PCState.Idle;

			for (int i = 0; i < m_PotentialTargets.Count; i++)
			{
				ChildToPotentialTarget = Utility.Distance(m_Child.transform.position, m_PotentialTargets[i].transform.position);
				TargetCurrentState = m_PotentialTargets[i].GetCurrentState();
				
				if (CheckIfTargetIsAvailable(m_PotentialTargets[i].gameObject) && ChildToPotentialTarget < fDistanceBetween && TargetCurrentState != PCState.Dead && TargetCurrentState != PCState.Avoid)
				{
					fDistanceBetween = ChildToPotentialTarget;
					m_TargetCell = m_PotentialTargets[i].gameObject;
					break;
				}
			}
			if(m_TargetCell != null){return m_TargetCell;}
		}
		//Else If the target to obtain a child cell to attack is the squad captain cell, loop through the cells within that squad to get the closest child cell to the enemy child cell
		else if(m_TargetSource.name.Contains("Squad"))
		{
			List<SquadChildFSM> m_PotentialTargets = SquadChildFSM.GetAliveChildList();
			if(m_PotentialTargets.Count <= 0)
			{
				return null;
			}

			SquadChildFSM ClosestSquadChild = m_PotentialTargets[0];
			float ChildToSquadChild = 0f;
			float ClosestDistance = Mathf.Infinity;

			for(int i = 0; i < m_PotentialTargets.Count; i++)
			{
				ChildToSquadChild = Utility.Distance(m_Child.transform.position,m_PotentialTargets[i].transform.position);
				if(ChildToSquadChild < ClosestDistance)
				{
					ClosestSquadChild = m_PotentialTargets[i];
					ClosestDistance = ChildToSquadChild;
				}
			}

			m_ecFSM.Target = ClosestSquadChild.gameObject;

			return m_ecFSM.Target;
		}

		//If there is no target for the child cell, transition it back to idle state
		return null;
	}

	//A function to evaluate the node based on the enemy main cell and player condition to return a score
	private int EvaluateNode (GameObject _Node)
	{
		//if the node contains no cell, it serve no threat to the enemy main cell
		if(_Node.GetComponent<Node_Manager>().activeChildCount == 0)
		{
			return 0;
		}

		int nthreatLevel = 0;

		//increase score based on amount of cells in that node
		nthreatLevel += _Node.GetComponent<Node_Manager>().activeChildCount;

		return nthreatLevel;
	}

	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		return (Utility.Distance(m_Child.transform.position,_Pos) <= 0.05f) ? true : false;
	}

	//A function that return a list of GameObjects that are within a circular range to the enemy child cell
	private List<GameObject> TagNeighbours()
	{
		List<GameObject> Neighbours = new List<GameObject>();
		Collider2D[] Neighbouring = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2, Constants.s_onlyEnemeyChildLayer);

		for(int i = 0; i < Neighbouring.Length; i++)
		{
			if(Neighbouring[i].gameObject != m_Child && Neighbouring[i].gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.ChargeChild)
			{
				Neighbours.Add(Neighbouring[i].gameObject);
			}
		}

		return Neighbours;
	}

	private void CheckIfEnoughCells()
	{
		if(m_ecFSM.m_AttackTarget.name.Contains("LeftNode") && ECTracker.s_Instance.ChargeChildCells.Count >= GameObject.Find("UI_Player_LeftNode").GetComponent<Node_Manager>().activeChildCount)
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0);
		}
		else if(m_ecFSM.m_AttackTarget.name.Contains("RightNode") && ECTracker.s_Instance.ChargeChildCells.Count >= GameObject.Find("UI_Player_RightNode").GetComponent<Node_Manager>().activeChildCount)
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0);
		}
		else if(m_ecFSM.m_AttackTarget.name.Contains("Squad") && ECTracker.Instance.ChargeChildCells.Count >= GameObject.Find("Squad_Captain_Cell").GetComponent<PlayerSquadFSM>().AliveChildCount())
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0);
		}
	}
	
	private IEnumerator SqueezeBeforeCharge(GameObject _Target)
	{
		//The child cell will retreat slightly back before charging 
		Vector3 ShrinkScale = new Vector3(0f,-0.1f,0f);
		Vector3 ExpandScale = new Vector3(0.1f,0f,0f);
		
		while(m_Child.transform.localScale.y > 0.5f)
		{
			Vector2 DirectionToTarget = -(_Target.transform.position - m_Child.transform.position).normalized;
			float Rotation = -Mathf.Atan2(DirectionToTarget.x, DirectionToTarget.y) * Mathf.Rad2Deg;
			m_ecFSM.transform.eulerAngles = new Vector3(0.0f,0.0f,Rotation);
			
			m_ecFSM.rigidbody2D.velocity = DirectionToTarget;
			
			m_Child.transform.localScale += ShrinkScale;
			m_Child.transform.localScale += ExpandScale;
			yield return new WaitForSeconds(0.2f);//0.0005
		}
		
		m_bSqueezeDone = true;
		AudioManager.PlayECSoundEffect(EnemyChildSFX.CellChargeTowards, m_ecFSM.Audio);
	}
}
