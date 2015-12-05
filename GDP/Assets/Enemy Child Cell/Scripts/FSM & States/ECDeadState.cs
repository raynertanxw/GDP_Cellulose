using UnityEngine;
using System.Collections;

public class ECDeadState : IECState {
	
	private Vector2 m_RespawnPos;
	private bool bDisable;
	
	public ECDeadState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.eMain;
		m_RespawnPos = new Vector2(0f,0f);
	}
	
	public override void Enter()
	{
		bDisable = true;
		DisableCell();
	}
	
	public override void Execute()
	{

	}
	
	public override void Exit()
	{
		EnableCell();
	}
	
	private void DisableCell()
	{
		m_Child.GetComponent<SpriteRenderer>().enabled = false;
		m_Child.GetComponent<BoxCollider2D>().enabled = false;
		m_Child.GetComponent<Rigidbody2D>().isKinematic = true;
		m_Child.transform.SetParent(m_Main.GetComponent<EnemyMainFSM>().Pool.transform);
		ECPoolManager.AddToPool(m_Child); //Add when pool is implemneted
	}
	
	private void EnableCell()
	{
		m_Child.GetComponent<SpriteRenderer>().enabled = true;
		m_Child.GetComponent<BoxCollider2D>().enabled = true;
		m_Child.GetComponent<Rigidbody2D>().isKinematic = false;
	}
}
