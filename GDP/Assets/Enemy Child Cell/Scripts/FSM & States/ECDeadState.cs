using UnityEngine;
using System.Collections;

public class ECDeadState : IECState {

	//Contructor
	public ECDeadState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
	}

	public override void Enter()
	{
		m_Child.transform.localScale = Vector3.one;
		DisableCell();
	}

	public override void Execute()
	{

	}

	public override void Exit()
	{
		//Enable the enemy child cell once it exits the dead state
		EnableCell();
	}

	//A function to disable the enemy child cells by disabling the visual and collisions of the child cell and add it back into the pool
	private void DisableCell()
	{
		m_Child.GetComponent<SpriteRenderer>().enabled = false;
		m_Child.GetComponent<BoxCollider2D>().enabled = false;
		m_ecFSM.rigidbody2D.isKinematic = true;
		ECPoolManager.AddToPool(m_Child);
	}

	//a function to enable the enemy child cell by re-enabling the visual and collision of the child cell
	private void EnableCell()
	{
		m_Child.GetComponent<SpriteRenderer>().enabled = true;
		m_Child.GetComponent<BoxCollider2D>().enabled = true;
		m_ecFSM.rigidbody2D.isKinematic = false;
	}
}
