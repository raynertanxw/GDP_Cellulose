using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyChildFSM : MonoBehaviour
{
	private int nIndex;
	public float fSpeed;
	private bool bIsMine;
	
	public GameObject pMain;
	public GameObject eMain;
	public GameObject chargeTarget;
	
	private IECState m_CurrentState;
	private ECState m_CurrentEnum;
	private MessageType m_CurrentCommand;
	private Dictionary<ECState,IECState> m_StatesDictionary;
	
	void Start()
	{
		fSpeed = 0.01f;
		bIsMine = false;
		pMain = GameObject.Find("Player_Cell");
		eMain = GameObject.Find("Enemy_Cell");
		chargeTarget = null;
		m_StatesDictionary = new Dictionary<ECState,IECState>();
		
		m_StatesDictionary.Add(ECState.Idle, new ECIdleState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Defend, new ECDefendState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Avoid, new ECAvoidState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Attack, new ECAttackState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.ChargeMain, new ECChargeMState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.ChargeChild, new ECChargeCState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.TrickAttack, new ECTrickAttackState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Landmine, new ECMineState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Dead, new ECDeadState(this.gameObject,this));
		
		m_CurrentState = m_StatesDictionary[ECState.Dead];
		m_CurrentEnum = ECState.Dead;
		m_CurrentCommand = MessageType.Empty;
		CurrentState.Enter();
	}
	
	void Update()
	{
		m_CurrentState.Execute();
		UpdateState();
	}
	
	public Dictionary<ECState,IECState> StateDictionary
	{
		get { return m_StatesDictionary; }
	}
	
	public int Index
	{
		get { return nIndex; }
		set { nIndex = value; }
	}
	
	public GameObject Target
	{
		get { return chargeTarget; }
		set { chargeTarget = value; }
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
	
	public void ChangeState(ECState _state)
	{
		m_CurrentState.Exit();
		m_CurrentState = m_StatesDictionary[_state];
		m_CurrentState.Enter();
		m_CurrentCommand = MessageType.Empty;
		m_CurrentEnum = _state;
	}
	
	private void UpdateState()
	{
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
	}
	
	public void KillChildCell()
	{
		ChangeState(ECState.Dead);
	}
	
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
	
	public void StartChildCorountine(IEnumerator _childCorountine)
	{
		StartCoroutine(_childCorountine);
	}
	
	public void StopChildCorountine(IEnumerator _childCorountine)
	{
		StopCoroutine(_childCorountine);
	}
	
	/*IEnumerator CountToDeath()
   {
       yield return new WaitForSeconds(10f);
       ChangeState(deadState);
   }*/
	
	/*private bool IsUnderAttack()
    {

    }*/
}
