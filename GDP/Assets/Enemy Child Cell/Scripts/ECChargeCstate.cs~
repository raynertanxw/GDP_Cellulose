using UnityEngine;
using System.Collections;

public class ECChargeCState : IECState {
	
	private float fChargeSpeed;
	public GameObject m_Target;
	
	public ECChargeCState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_ecFSM = _ecFSM;
		m_Child = _childCell;
		fChargeSpeed = 0.3f;
	}
	
	public override void Enter()
	{
		m_Target = FindTargetChild();
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
	}
	
	public override void Execute()
	{
		if (!HasCellReachTargetPos(m_Target.transform.position))
		{
			ChargeTowards(m_Target);
		}
		else
		{
			m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
			MessageDispatcher.Instance.DispatchMessage(m_Child, m_ecFSM.gameObject, MessageType.Dead, 0.0);
		}
	}
	
	public override void Exit()
	{
	}
	
	private bool CheckIfTargetIsAvailable(GameObject _GO)
	{
		GameObject[] childCells = GameObject.FindGameObjectsWithTag("EnemyChild");
		for (int i = 0; i < childCells.Length; i++)
		{
			if (childCells[i].GetComponent<ECChargeCState>().m_Target == _GO)
			{
				return false;
			}
		}
		return true;
	}
	
	
	private GameObject FindTargetChild()
	{
		GameObject[] childCells = GameObject.FindGameObjectsWithTag("PlayerChild");
		float distanceBetween = Mathf.Infinity;
		GameObject closestChild = childCells[0];
		
		for (int i = 0; i < childCells.Length; i++)
		{
			if (CheckIfTargetIsAvailable(childCells[i]) && (Vector2.Distance(m_Child.transform.position, childCells[i].transform.position)) < distanceBetween)
			{
				distanceBetween = Vector2.Distance(m_Child.transform.position, childCells[i].transform.position);
				closestChild = childCells[i];
			}
		}
		
		return closestChild;
	}
	
	
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2 + m_Target.GetComponent<SpriteRenderer>().bounds.size.x/2)
		{
			return true;
		}
		return false;
	}
	
	private void ChargeTowards(GameObject _PC)
	{
		Vector2 m_TargetPos = _PC.transform.position;
		Vector2 m_Difference = new Vector2(m_TargetPos.x - m_Child.transform.position.x, m_TargetPos.y - m_Child.transform.position.y);
		Vector2 m_Direction = m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fChargeSpeed;
	}
}
