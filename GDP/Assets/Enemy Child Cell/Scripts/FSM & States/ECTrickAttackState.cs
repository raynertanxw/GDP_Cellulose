using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECTrickAttackState : IECState {

	private GameObject m_AttackTarget;
	private Vector2 m_StartTelePos;
	private Vector2 m_EndTelePos;
	private Vector2 m_TargetPos;
	private bool bReachStart;
	private float fChargeSpeed;
	private GameObject[] m_Nodes;

    public ECTrickAttackState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.eMain;
		m_Nodes = new GameObject[3];
    }
    
    public override void Enter()
    {
		m_Nodes[0] = GameObject.Find("Node_Left");
		m_Nodes[1] = GameObject.Find("Node_Top");
		m_Nodes[2] = GameObject.Find("Node_Right");
		m_AttackTarget = GetTarget();
		bReachStart = false;
		fChargeSpeed = 1f;
		
		List<Vector2> m_Positions = CalculateKeyPositions();
		m_StartTelePos = m_Positions[0];
		m_EndTelePos = m_Positions[1];
		m_TargetPos = m_Positions[2];
    }

    public override void Execute()
    {
		if(bReachStart == false)
		{
			ChargeTowards(m_StartTelePos);
			if(HasCellReachTargetPos(m_StartTelePos))
			{
				m_Child.transform.position = m_EndTelePos;
				bReachStart = true;
			}
		}
		else
		{
			ChargeTowards(m_TargetPos);
		}
    }

    public override void Exit()
    {

    }
    
    private bool IsAllNodesEmpty()
    {
		for(int i = 0; i < m_Nodes.Length; i++)
		{
			if(m_Nodes[i].GetComponent<Node_Manager>().GetNodeChildList().Count > 0)
			{
				return false;
			}
		}
		
		return true;
    }
    
    private GameObject GetWeakestNode()
    {
		int[] m_Scores = new int[3];
		m_Scores[0] = EvaluteNode(m_Nodes[0]);
		m_Scores[1] = EvaluteNode(m_Nodes[1]);
		m_Scores[2] = EvaluteNode(m_Nodes[2]);
		
		int nIndex = 0;
		int nLowestScore = 999;
		for(int i = 0; i < m_Scores.Length; i++)
		{
			if(m_Scores[i] < nLowestScore)
			{
				nIndex = i;
				nLowestScore = m_Scores[i];
			}
		}
		
		return m_Nodes[nIndex];
    }
    
    private int EvaluteNode(GameObject _Node)
    {
		//if the node contains no cell, it serve no threat to the enemy main cell
		if(_Node.GetComponent<Node_Manager>().GetNodeChildList().Count == 0)
		{
			return 0;
		}
		
		int nthreatLevel = 0;
		
		//increase score based on amount of cells in that node
		nthreatLevel += _Node.GetComponent<Node_Manager>().GetNodeChildList().Count;
		
		//increase score if that node have formed together and has a node captain
		if(_Node.GetComponent<SquadCaptain>() != null)
		{
			nthreatLevel+= 50;
			
			//increase score by the amount of nutrients that node has
			
		}
		
		return nthreatLevel;
    }
    
    private GameObject GetTarget()
    {
		if(IsAllNodesEmpty())
		{
			return m_ecFSM.pMain;
		}
		else
		{
			return GetWeakestNode();
		}
    } 
    
    private List<Vector2> CalculateKeyPositions()
    {
		List<Vector2> Positions = new List<Vector2>();
		
		//Start Tele Position//
		//Generate a random Y to start teleporting
		float fYLimit = (m_Main.transform.position.y + m_ecFSM.pMain.transform.position.y) / 2;
		float fInitialY = Random.Range(fYLimit/2,fYLimit);
		
		//find a side of the Wall to teleport from
		GameObject m_InitialWall = GetClosestWall(m_Child.transform.position);
		
		//calculate the appropriate position to start teleporting 
		Vector2 m_StartPos = new Vector2(m_InitialWall.transform.position.x, fInitialY);
		
		//End Tele Position//
		//Use the corresponding Y from start tele or decrease it for the end tele position
		float Noise = Random.Range (-0.5f,0f);
		float fNextY = fInitialY + Noise;
		
		//Use the opposide side wall from the previous one
		GameObject m_NextWall = null;
		if(m_InitialWall == GameObject.Find("Left Wall"))
		{
			m_NextWall = GameObject.Find("Right Wall");
		}
		else
		{
			m_NextWall = GameObject.Find("Left Wall");
		}
		
		//Generate the appropriate position to appear from the teleport
		Vector2 m_EndPos = new Vector2(m_NextWall.transform.position.x, fNextY);
		
		Positions.Add(m_StartPos);
		Positions.Add(m_EndPos);
		Positions.Add(m_AttackTarget.transform.position);
		
		return Positions;
    }
    
    private GameObject GetClosestWall (Vector2 _Pos)
    {
		GameObject m_LWall = GameObject.Find("Left Wall");
		GameObject m_RWall = GameObject.Find("Right Wall");
		
		if(Vector2.Distance(_Pos, m_LWall.transform.position) < Vector2.Distance(_Pos, m_RWall.transform.position))
		{
			return m_RWall;
		}
		return m_LWall;
    }
    
	private void ChargeTowards(Vector2 _Pos)
	{
		Vector2 m_TargetPos = _Pos;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fChargeSpeed;
		fChargeSpeed += 0.2f;
		fChargeSpeed = Mathf.Clamp(fChargeSpeed,1f,12f);
	}
	
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2 + GameObject.Find("Left Wall").GetComponent<SpriteRenderer>().bounds.size.x/2)
		{
			return true;
		}
		return false;
	}
}
