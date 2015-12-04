using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECChargeCState : IECState {
	
	private float fChargeSpeed;
	
	public ECChargeCState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.eMain;
		m_Child = _childCell;
		fChargeSpeed = 0.3f;
	}
	
	public override void Enter()
	{
		//Debug.Log("Enter charge child");
		m_ecFSM.chargeTarget = FindTargetChild();
	}
	
	public override void Execute()
	{
		if(!HasCellReachTargetPos(m_ecFSM.chargeTarget.transform.position))
		{
			ChargeTowards(m_ecFSM.chargeTarget);
		}
		else
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child.gameObject, m_Child.gameObject, MessageType.Dead, 0);
		}
	}
	
	public override void Exit()
	{
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
	}
	
	private bool CheckIfTargetIsAvailable(GameObject _Target)
	{
		GameObject[] m_Childs = GameObject.FindGameObjectsWithTag(Constants.s_strEnemyChildTag);
		return null;
	}
	
	private GameObject FindTargetChild()
	{
		//Debug.Log("find target child");
	
		Squad_Manager m_TargetSquad = GetMostThreateningSquad();
		List<PlayerChildFSM> m_PotentialTargets = m_TargetSquad.GetSquadChildList();
		
		//Debug.Log("step 1");
		
		float fDistanceBetween = Mathf.Infinity;
		int nAvaliableEnemyChildCells = m_Main.GetComponent<EnemyMainFSM>().ECList.Count;
		GameObject m_TargetCell = m_PotentialTargets[0].gameObject;
		
		//Debug.Log("step 2");
		
		for (int i = 0; i < m_PotentialTargets.Count; i++)
		{
			//Debug.Log("step 2.1");
			if (CheckIfTargetIsAvailable(m_PotentialTargets[i].gameObject) && (Vector2.Distance(m_Child.transform.position, m_PotentialTargets[i].transform.position)) < fDistanceBetween)
			{
				//Debug.Log("step 2.2");
				fDistanceBetween = Vector2.Distance(m_Child.transform.position, m_PotentialTargets[i].transform.position);
				m_TargetCell = m_PotentialTargets[i].gameObject;
			}
		}
		
		//Debug.Log("step 3");
		
		Utility.CheckEmpty<GameObject>(m_TargetCell);
		
		//Debug.Log("step 4");
		
		//If there is no more available player child to be targeted, stop this state and shift back to idle state
		if(m_TargetCell == null)
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child, m_Child, MessageType.Idle, 0);
			//Debug.Log("No Target Avaiable");
			return null;
		}
		
		//Debug.Log("step 5");
		
		return m_TargetCell;
	}
	
	private Squad_Manager GetMostThreateningSquad()
	{
		//3 different player squads
		GameObject m_TopSquad = GameObject.Find("Squad_Top");
		GameObject m_LeftSquad = GameObject.Find("Squad_Left");
		GameObject m_RightSquad = GameObject.Find("Squad_Right");
		
		//Scores for the 3 different squads
		int nTopScore = EvaluateSquad(m_TopSquad);
		int nLeftScore = EvaluateSquad(m_LeftSquad);
		int nRightScore = EvaluateSquad(m_RightSquad);
		
		int nHighestThreat = Mathf.Max(Mathf.Max(nTopScore,nLeftScore),nRightScore);
		
		if(nHighestThreat == nTopScore)
		{
			return m_TopSquad.GetComponent<Squad_Manager>();
		}
		else if(nHighestThreat == nLeftScore)
		{
			return m_LeftSquad.GetComponent<Squad_Manager>();
		}
		else if(nHighestThreat == nRightScore)
		{
			return m_RightSquad.GetComponent<Squad_Manager>();
		}
		
		return null;
	}
	
	private int EvaluateSquad (GameObject _Squad)
	{
		//if the squad contains no cell, it serve no threat to the enemy main cell
		if(_Squad.GetComponent<Squad_Manager>().GetSquadChildList().Count == 0)
		{
			return 0;
		}
		
		int nthreatLevel = 0;
		
		//increase score based on amount of cells in that squad
		nthreatLevel += _Squad.GetComponent<Squad_Manager>().GetSquadChildList().Count;
	
		//increase score if that squad have formed together and has a squad captain
		if(_Squad.GetComponent<SquadCaptain>() != null)
		{
			nthreatLevel+= 50;
			
			//increase score by the amount of nutrients that squad has
			
		}
		
		return nthreatLevel;
	}
	
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2 + m_ecFSM.chargeTarget.GetComponent<SpriteRenderer>().bounds.size.x/2)
		{
			return true;
		}
		return false;
	}
	
	private void ChargeTowards(GameObject _PC)
	{
		Vector2 m_TargetPos = _PC.transform.position;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fChargeSpeed;
		fChargeSpeed += 0.2f;
		fChargeSpeed = Mathf.Clamp(fChargeSpeed,1f,12f);
	}
}
