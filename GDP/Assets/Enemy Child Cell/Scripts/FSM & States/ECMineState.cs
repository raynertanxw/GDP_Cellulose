using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECMineState : IECState {

	private Vector2 m_TargetPosition;
	private bool bReachPosition;
	private bool bExplosionStart;
	private float fSpeed;

    public ECMineState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.eMain;
    }

    public override void Enter()
    {
		Debug.Log("Enter landmine");
		PointDatabase.Instance.RefreshDatabase(m_Main.transform.position, m_ecFSM.pMain.transform.position ,GameObject.Find("Left Wall"));
		m_TargetPosition = PositionQuery.Instance.RequestLandminePos(DetermineType(), m_Main, m_ecFSM.pMain);
		fSpeed = 12f;
		bReachPosition = false;
		bExplosionStart = false;
    }

    public override void Execute()
    {
		if(!HasCellReachTarget(m_TargetPosition) && bReachPosition == false)
		{
			FloatTowards(m_TargetPosition);
		}
		else if(HasCellReachTarget(m_TargetPosition) && bReachPosition == false)
		{
			bReachPosition = true;
			FloatForward();
		}
		
		if(IsCollidingWithPlayerCell() && bExplosionStart == false)
		{
			m_ecFSM.StartChildCorountine(ExplodeCorountine());
			bExplosionStart = true;
		}
		
		if(bExplosionStart == true && HasCollidedWithPlayerCells())
		{
			ExplodeDestroy();
		}
    }

    public override void Exit()
    {
		Debug.Log("Exit landmine");
    }
    
    private PositionType DetermineType()
    {
		float fEMainAggressiveness = m_Main.GetComponent<EnemyMainFSM>().CurrentAggressiveness;

		if(fEMainAggressiveness >= 12)
		{
			Debug.Log("Aggressive");
			return PositionType.Aggressive;
		}
		else if(fEMainAggressiveness <= 6)
		{
			Debug.Log("Defensive");
			return PositionType.Defensive;
		}
		Debug.Log("Neutral");
		return PositionType.Neutral;
    }
	
	private void FloatTowards(Vector2 _TargetPos)
	{
		Vector2 m_TargetPos = _TargetPos;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fSpeed;
		fSpeed -= 0.2f;
		fSpeed = Mathf.Clamp(fSpeed,1f,6f);
	}
	
	private void FloatForward()
	{
		m_Child.GetComponent<Rigidbody2D>().velocity = Vector2.down;
	}
	
	private bool HasCellReachTarget (Vector2 _TargetPos)
	{
		if (Vector2.Distance(m_Child.transform.position, _TargetPos) <= 0.1f)
		{
			return true;
		}
		return false;
	}
	
	private bool IsCollidingWithPlayerCell()
	{
		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position, 1.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		
		if(m_SurroundingObjects.Length <= 0)
		{
			return false;
		}
		
		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			if(m_SurroundingObjects[i] != null && (m_SurroundingObjects[i].tag == Constants.s_strPlayerChildTag || m_SurroundingObjects[i].tag == Constants.s_strPlayerTag))
			{
				return true;
			}
		}
		
		return false;
	}
	
	private bool HasCollidedWithPlayerCells()
	{
		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position, 0.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		if(m_SurroundingObjects.Length <= 0)
		{
			return false;
		}
		
		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			if(m_SurroundingObjects[i] != null && (m_SurroundingObjects[i].tag == Constants.s_strPlayerChildTag || m_SurroundingObjects[i].tag == Constants.s_strPlayerTag))
			{
				return true;
			}
		}
		
		return false;
	}
	
	private void ExplodeSetup()
	{
		
	}
	
	private void ExplodeDestroy()
	{
		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position,1.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			//if the player child cell is within the exploding range, kill the player child
			if(m_SurroundingObjects[i].gameObject.tag == Constants.s_strPlayerChildTag)
			{
				//kill the player child
			}
			//if the player main cell is within the exploding range, damage the player main
			else if(m_SurroundingObjects[i].gameObject.tag == Constants.s_strPlayerTag)
			{
				//damage the player main cell
			}
		}
		MessageDispatcher.Instance.DispatchMessage(m_Child, m_Child,MessageType.Dead,0);
	}
	
	IEnumerator ExplodeCorountine()
	{
		//play explode sound/animation whatever
		ExplodeSetup();
		
		yield return new WaitForSeconds(1.5f);
		
		ExplodeDestroy();
	}
}
