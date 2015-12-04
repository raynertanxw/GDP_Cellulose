using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECAvoidState : IECState {

	private List<GameObject> m_Attackers;

    // Use this for initialization
    public ECAvoidState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
    }

    public override void Enter()
    {

    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }

	private List<GameObject> ReturnAttackersNearby()
	{
		Collider2D[] Attackers = Physics2D.OverlapCircleAll(m_Child.transform.position, 3 * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		List<GameObject> PlayerChildAttacking = new List<GameObject>();

		for(int i = 0; i < Attackers.Length; i++)
		{
			if(Attackers[i].gameObject.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeMain)
			{
				PlayerChildAttacking.Add(Attackers[i].gameObject);
			}
		}

		return PlayerChildAttacking;
	}

	private void MoveTowards(Vector2 _Pos)
	{

	}

    private Vector2 Avoid()
	{
		m_Attackers = ReturnAttackersNearby();

		int AttackerCount = 0;
		Vector2 Steering = new Vector2(0f,0f);

		foreach(GameObject Attacker in m_Attackers)
		{
			if(Attacker != null)
			{
				Steering.x += Attacker.transform.position.x;
				Steering.y += Attacker.transform.position.y;
				AttackerCount++;
			}
		}

		if(AttackerCount <= 0)
		{
			return Steering;
		}
		else
		{
			Steering /= AttackerCount;
			Steering = new Vector2(Steering.x - m_Child.transform.position.x, Steering.y - m_Child.transform.position.y);
			Steering.Normalize();
			return Steering;
		}

	}

}
