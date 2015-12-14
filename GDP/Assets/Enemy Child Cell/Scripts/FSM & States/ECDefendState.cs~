using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECDefendState : IECState {
	
	//a boolean to track whether the position of the enemy child cell had reach the guiding position for 
	//the defence formation
	private bool bReachPos;
	
	//a float to store the movement speed of the enemy child towards the defending position
	private float fMoveSpeed;
	
	//a vector2 to store the defending position that the enemy child cell need to move to
    private Vector2 m_TargetPos;
    
    //a static vector to store the central positon of the defending position for all defending enemy child cells
    private static Vector2 s_m_FormationCenter;
    
    private float fDefendTime;

	//Constructor
    public ECDefendState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = _ecFSM.m_EMain;
		fMoveSpeed = 3f;
		fDefendTime = 0f;
    }

    public override void Enter()
    {
		bReachPos = false;
		fDefendTime = 0f;
    }

    public override void Execute()
    {
		//Update the center point of defensive formation
		s_m_FormationCenter = new Vector2(m_Main.transform.position.x, m_Main.transform.position.y - (m_Child.GetComponent<SpriteRenderer>().bounds.size.y + m_Main.GetComponent<SpriteRenderer>().bounds.size.y));
   
		//if the enemy child cell had not reach the central positon of the defending position, move the enemy
		//child cell towards the target position
		if(!HasCellReachTargetPos(s_m_FormationCenter) && bReachPos == false)
		{
			MoveTowards(s_m_FormationCenter);
		}		
		else if(HasCellReachTargetPos(s_m_FormationCenter) && bReachPos == false)
		{
			bReachPos = true;
		}
	
		//if the enemy child cell had reached the central positon of the defending position, move towards 
		//and maintain its own defending position in the formation
		if(bReachPos == true)
		{
			Vector2 m_SteeringVelo = SpreadAcrossLine();
			m_SteeringVelo.x += m_Main.GetComponent<Rigidbody2D>().velocity.x;
			m_SteeringVelo.y += m_Main.GetComponent<Rigidbody2D>().velocity.y;
			m_Child.GetComponent<Rigidbody2D>().velocity = m_SteeringVelo;
		}
		
		if(IsThereNoAttackers())
		{
			fDefendTime += Time.deltaTime;
			if(fDefendTime > 2.0f)
			{
				MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0f);
			}
		}
    }

    public override void Exit()
    {
		fDefendTime = 0f;
    }
    
    public bool ReachPos
    {
		get { return bReachPos; }
    }
    
	//a function that direct the enemy child cell towards a gameObject by changing its velocity through calculation
	private void MoveTowards(Vector2 _targetPos)
	{
		Vector2 m_TargetPos = _targetPos;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fMoveSpeed;
	}

	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= m_Child.GetComponent<SpriteRenderer>().bounds.size.x)
		{
			return true;
		}
		return false;
	}

	//a function to return a velocity that direct the enemy child cell to spread from each other to from a defensive 
	//line in front of the enemy main cell
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
    
    private bool IsThereNoAttackers()
	{
		GameObject[] PlayerChilds = GameObject.FindGameObjectsWithTag(Constants.s_strPlayerChildTag);
		foreach(GameObject child in PlayerChilds)
		{
			if(child.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeChild || child.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeMain)
			{
				return false;
			}
		}
		return true;
	}
}
