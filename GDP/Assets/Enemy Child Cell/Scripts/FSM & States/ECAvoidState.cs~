using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECAvoidState : IECState {

	//A list of gameobjects that is used to store all the attacking player child cell towards the Enemy Child
	//Cell
	private List<GameObject> m_Attackers;

    //Constructor
    public ECAvoidState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
    }
    
	public override void Enter()
	{
		
	}
	
    public override void Execute()
    {
		//Calculate a target velocity for the Enemy Child cell to avoid any near player attacking cell
		Vector2 targetVelo = Avoid();
		
		//Add the velocity of the enemy main cell in order to keep up with it
		targetVelo.x += m_Main.GetComponent<Rigidbody2D>().velocity.x;
		targetVelo.y += m_Main.GetComponent<Rigidbody2D>().velocity.y;
		
		//Set the enemy child velocity to the target velocity
		m_Child.GetComponent<Rigidbody2D>().velocity = targetVelo;
    }

    public override void Exit()
    {
		
    }

	//Return a list of attacking player child cells that are nearby
	private List<GameObject> ReturnAttackersNearby()
	{
		Collider2D[] Attackers = Physics2D.OverlapCircleAll(m_Child.transform.position, 3 * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		List<GameObject> PlayerChildAttacking = new List<GameObject>();

		for(int i = 0; i < Attackers.Length; i++)
		{
			if(Attackers[i].gameObject.tag == Constants.s_strPlayerChildTag && Attackers[i].gameObject.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeMain)
			{
				PlayerChildAttacking.Add(Attackers[i].gameObject);
			}
		}

		return PlayerChildAttacking;
	}

	//A function that return a velocity to drive the child cell to avoid any attacking player cells nearby
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
