using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyChildFSM : MonoBehaviour
{
	//2 variables to store the speed of the enemy child cell and whether the child cell is in landmine state
	public float fSpeed;
	private bool bIsMine;
	
	//3 gameobjects variables to store the player main cell, enemy main cell and the target for this child
	//cell to charge towards
	public GameObject m_PMain;
	public GameObject m_EMain;
	public GameObject m_ChargeTarget;
	
	//3 variables to store the current state, the enumeration of the current state and the current command for
	//the child cell
	private IECState m_CurrentState;
	private ECState m_CurrentEnum;
	private MessageType m_CurrentCommand;
	
	//declare a dictoary to store various IECstate with the key being ECState
	private Dictionary<ECState,IECState> m_StatesDictionary;
	
	void Start()
	{
		//Initialize the variables and data structure
		fSpeed = 0.01f;
		bIsMine = false;
		m_PMain = GameObject.Find("Player_Cell");
		m_EMain = GameObject.Find("Enemy_Cell");
		m_ChargeTarget = null;
		m_StatesDictionary = new Dictionary<ECState,IECState>();
		
		//Initialize the various states for the enemy child cell and added them into the dictionary 
		m_StatesDictionary.Add(ECState.Idle, new ECIdleState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Defend, new ECDefendState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Avoid, new ECAvoidState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Attack, new ECAttackState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.ChargeMain, new ECChargeMState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.ChargeChild, new ECChargeCState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.TrickAttack, new ECTrickAttackState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Landmine, new ECMineState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Dead, new ECDeadState(this.gameObject,this));
		
		//initialize the current state for the enemy child cell
		m_CurrentState = m_StatesDictionary[ECState.Dead];
		m_CurrentEnum = ECState.Dead;
		m_CurrentCommand = MessageType.Empty;
		CurrentState.Enter();
	}
	
	void Update()
	{
		m_CurrentState.Execute();
		UpdateState();
		
		if(m_CurrentEnum == ECState.Idle && IsMainBeingAttacked() && !IsThereEnoughDefence())
		{
			Debug.Log("AutoDefend");
			AutoDefend();
		}	
	}
	
	//Various getter functionss
	public Dictionary<ECState,IECState> StateDictionary
	{
		get { return m_StatesDictionary; }
	}
	
	public GameObject Target
	{
		get { return m_ChargeTarget; }
		set { m_ChargeTarget = value; }
	}
	
	public MessageType Command
	{
		get { return m_CurrentCommand; }
		set { m_CurrentCommand = value; }
	}
	
	public IECState CurrentState
	{
		get { return m_CurrentState; }
		
	}
	
	public ECState CurrentStateEnum
	{
		get { return m_CurrentEnum; }
	}
	
	//a function to change the enemy child state and make the appropriate changes to the enemy child cell
	public void ChangeState(ECState _state)
	{
		m_CurrentState.Exit();
		m_CurrentState = m_StatesDictionary[_state];
		m_CurrentState.Enter();
		m_CurrentEnum = _state;
	}
	
	//a function to update the enemy child state based on the currentcommand variable in the enemy child FSM
	private void UpdateState()
	{
		if(m_CurrentCommand == MessageType.Empty)
		{
			return;
		}
	
		if (m_CurrentCommand == MessageType.Avoid)
		{
			ChangeState(ECState.Avoid);
		}
		else if (m_CurrentCommand == MessageType.Attack)
		{
			ChangeState(ECState.Attack);
		}
		else if(m_CurrentCommand == MessageType.ChargeChild)
		{
			ChangeState(ECState.ChargeChild);
		}
		else if(m_CurrentCommand == MessageType.ChargeMain)
		{
			ChangeState(ECState.ChargeMain);
		}
		else if(m_CurrentCommand == MessageType.TrickAttack)
		{
			ChangeState(ECState.TrickAttack);
		}
		else if (m_CurrentCommand == MessageType.Dead)
		{
			ChangeState(ECState.Dead);
		}
		else if (m_CurrentCommand == MessageType.Defend)
		{
			ChangeState(ECState.Defend);
		}
		else if (m_CurrentCommand == MessageType.Idle)
		{
			ChangeState(ECState.Idle);
		}
		else if (m_CurrentCommand == MessageType.Landmine)
		{
			ChangeState(ECState.Landmine);
		}
		
		m_CurrentCommand = MessageType.Empty;
	}
	
	//a function for player cells to kill this child cell by changing it to the dead state
	public void KillChildCell()
	{
		ChangeState(ECState.Dead);
	}
	
	//a function to return the amount of enemy child cell with the same state with this current state
	public int AmountOfSameCellState()
	{
		GameObject[] childCells = GameObject.FindGameObjectsWithTag("EnemyChild");
		int count = 0;
		for (int i = 0; i < childCells.Length; i++)
		{
			if (m_CurrentState == childCells[i].GetComponent<EnemyChildFSM>().CurrentState)
			{
				count++;
			}
		}
		return count;
	}
	
	private bool IsMainBeingAttacked()
	{
		Collider2D[] IncomingToMain = Physics2D.OverlapCircleAll(m_EMain.transform.position, 2 * m_EMain.GetComponent<SpriteRenderer>().bounds.size.x);
		foreach(Collider2D comingObject in IncomingToMain)
		{
			if(comingObject.tag == Constants.s_strPlayerChildTag)
			{
				Debug.Log("main detect");
				return true;
			}
		}
	
		Collider2D[] IncomingToChild = Physics2D.OverlapCircleAll(gameObject.transform.position, 12 * GetComponent<SpriteRenderer>().bounds.size.x);
		
		foreach(Collider2D comingObject in IncomingToChild)
		{
			if(comingObject.tag == Constants.s_strPlayerChildTag && comingObject.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeMain)
			{
				Debug.Log("child detect");
				return true;
			}
		}
		
		return false;
	}
	
	private bool IsThereEnoughDefence()
	{
		GameObject[] Attackers = GameObject.FindGameObjectsWithTag(Constants.s_strPlayerChildTag);
		List<EnemyChildFSM> Child = m_EMain.GetComponent<EnemyMainFSM>().ECList;
		int attackerAmount = 0;
		int defenderAmount = 0;
		
		foreach(GameObject attacker in Attackers)
		{
			if(attacker.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeChild || attacker.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeMain)
			{
				attackerAmount++;
			}
		}
		
		foreach(EnemyChildFSM defender in Child)
		{
			if(defender.CurrentStateEnum == ECState.Defend)
			{
				defenderAmount++;
			}
		}
		
		if(attackerAmount > defenderAmount)
		{
			Debug.Log("Attacker more than defender");
			return false;
		}
		Debug.Log("defender more than Attacker");
		return true;
	}
	
	private void AutoDefend()
	{
		Collider2D[] NearbyObjects = Physics2D.OverlapCircleAll(gameObject.transform.position, 3 * GetComponent<SpriteRenderer>().bounds.size.x/2);
		
		//Dispatch a message to all nearby enemy child cells that are idling to defend the main cell
		foreach(Collider2D nearby in NearbyObjects)
		{
			if(nearby.tag == Constants.s_strEnemyChildTag && nearby.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle)
			{
				MessageDispatcher.Instance.DispatchMessage(gameObject,nearby.gameObject,MessageType.Defend,0);
			}
		}
	}
	
	//two functions to start and stop corountines that are called from the child states
	public void StartChildCorountine(IEnumerator _childCorountine)
	{
		StartCoroutine(_childCorountine);
	}
	
	public void StopChildCorountine(IEnumerator _childCorountine)
	{
		StopCoroutine(_childCorountine);
	}

	public IEnumerator PassThroughDeath()
	{
		GetComponent<Rigidbody2D>().velocity = new Vector2(0f,GetComponent<Rigidbody2D>().velocity.y);
		yield return new WaitForSeconds(1f);
		MessageDispatcher.Instance.DispatchMessage(this.gameObject,this.gameObject,MessageType.Dead,0);
		Debug.Log("EC Kill Self: " + gameObject.name);
	}
}
