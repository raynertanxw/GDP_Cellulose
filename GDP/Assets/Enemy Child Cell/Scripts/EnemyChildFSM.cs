﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyChildFSM : MonoBehaviour
{
    private int nIndex;
    public float fSpeed;
    private bool bIsMine;
    public GameObject pMain;
    public GameObject eMain;
    private IECState m_CurrentState;
    private MessageType m_CurrentCommand;
    private Dictionary<ECState,IECState> m_StatesDictionary;

    void Start()
    {
        fSpeed = 0.01f;
        bIsMine = false;
        pMain = GameObject.Find("Player_Cell");
        eMain = GameObject.Find("Enemy_Cell");
		m_StatesDictionary = new Dictionary<ECState,IECState>();
        
        m_StatesDictionary.Add(ECState.Idle, new ECIdleState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Defend, new ECIdleState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Avoid, new ECIdleState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Attack, new ECIdleState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.ChargeMain, new ECIdleState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.ChargeChild, new ECIdleState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.TrickAttack, new ECIdleState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Landmine, new ECIdleState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Dead, new ECIdleState(this.gameObject,this));
		
		m_CurrentState = m_StatesDictionary[ECState.Dead];
		m_CurrentCommand = MessageType.Idle;
    }

    void Update()
    {
		m_CurrentState.Execute();
        UpdateState();
    }

    public int Index
    {
        get { return nIndex; }
        set { nIndex = value; }
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

    public void ChangeState(IECState _state)
    {
		m_CurrentState.Exit();
		m_CurrentState = _state;
		m_CurrentState.Enter();
		m_CurrentCommand = MessageType.Empty;
    }

    private void UpdateState()
    {
		if (m_CurrentCommand == MessageType.Avoid)
        {
			ChangeState(m_StatesDictionary[ECState.Avoid]);
        }
		else if (m_CurrentCommand == MessageType.Attack)
        {
			ChangeState(m_StatesDictionary[ECState.Attack]);
        }
		else if (m_CurrentCommand == MessageType.Dead)
        {
			ChangeState(m_StatesDictionary[ECState.Dead]);
        }
		else if (m_CurrentCommand == MessageType.Defend)
        {
			ChangeState(m_StatesDictionary[ECState.Defend]);
        }
		else if (m_CurrentCommand == MessageType.Idle)
        {
			ChangeState(m_StatesDictionary[ECState.Idle]);
        }
		else if (m_CurrentCommand == MessageType.Landmine)
        {
			ChangeState(m_StatesDictionary[ECState.Landmine]);
        }
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
