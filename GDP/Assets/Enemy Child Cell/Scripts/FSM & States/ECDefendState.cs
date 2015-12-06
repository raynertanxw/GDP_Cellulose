using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECDefendState : IECState {
	
	private bool bReachPos;
	private float fMoveSpeed;
    private Vector2 m_TargetPos;
    private static Vector2 s_m_FormationCenter;

    public ECDefendState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = _ecFSM.eMain;
		fMoveSpeed = 3f;
    }

    public override void Enter()
    {
		bReachPos = false;
    }

    public override void Execute()
    {
		//Update the center point of defensive formation
		s_m_FormationCenter = new Vector2(m_Main.transform.position.x, m_Main.transform.position.y - (m_Child.GetComponent<SpriteRenderer>().bounds.size.y + m_Main.GetComponent<SpriteRenderer>().bounds.size.y));
   
		if(!HasCellReachTargetPos(s_m_FormationCenter) && bReachPos == false)
		{
			MoveTowards(s_m_FormationCenter);
		}		
		else if(HasCellReachTargetPos(s_m_FormationCenter) && bReachPos == false)
		{
			bReachPos = true;
			//Debug.Log("Reach");
		}
	
		if(bReachPos == true)
		{
			Vector2 m_SteeringVelo = SpreadAcrossLine();
			m_SteeringVelo.x += m_Main.GetComponent<Rigidbody2D>().velocity.x;
			m_SteeringVelo.y += m_Main.GetComponent<Rigidbody2D>().velocity.y;
			m_Child.GetComponent<Rigidbody2D>().velocity = m_SteeringVelo;
		}
		
    }

    public override void Exit()
    {

    }
    
    public bool ReachPos
    {
		get { return bReachPos; }
    }

	private void MoveTowards(Vector2 _targetPos)
	{
		Vector2 m_TargetPos = _targetPos;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fMoveSpeed;
	}

	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= m_Child.GetComponent<SpriteRenderer>().bounds.size.x)
		{
			return true;
		}
		return false;
	}

    private Vector2 SpreadAcrossLine()
    {
		Collider2D[] m_NeighbourChilds = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2);//Physics2D.OverlapAreaAll(m_SpreadTopLeft,m_SpreadBotRight,LayerMask.NameToLayer ("EnemyChild"));
		List<GameObject> m_DefendingChilds = new List<GameObject>();
		
		for(int i = 0; i < m_NeighbourChilds.Length; i++)
		{
			if(m_NeighbourChilds[i] != null && m_NeighbourChilds[i].tag == Constants.s_strEnemyChildTag && m_NeighbourChilds[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Defend)
			{
				m_DefendingChilds.Add(m_NeighbourChilds[i].gameObject);
			}
		}
		
		int nDefendingCount = 0;
		Vector2 m_Steering = new Vector2(0f,0f);
		foreach(GameObject child in m_DefendingChilds)
		{
			if(child != null && child != m_Child)
			{
				m_Steering.x += child.transform.position.x - m_Child.transform.position.x;
				//m_Steering.y += child.transform.position.y - m_Child.transform.position.y;
				nDefendingCount++;
			}
		}
		
		if (nDefendingCount <= 0)
		{
			return m_Steering;
		}
		else
		{
			m_Steering /= nDefendingCount;
			m_Steering *= -1f;
			m_Steering.Normalize();
			return m_Steering;
		}
    }
}
