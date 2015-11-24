using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyChildFSM : MonoBehaviour {

    private int nIndex;
    public float fSpeed;
    private bool bIsMine;
    public GameObject pMain;
    public GameObject eMain;
    private IECState currentState;
    private ECIdleState idleState;
    private ECDefendState defendState;
    private ECAvoidState avoidState;
    private ECAttackState attackState;
    private ECChargeCState chargeCState;
    private ECChargeMState chargeMState;
    private ECTrickAttackState tAttackState;
    private ECDeadState deadState;
    private ECMineState mineState;

    private ECStatus type;
    private MessageType currentCommand;

    void Start()
    {
        fSpeed = 0.01f;
        bIsMine = false;
        pMain = GameObject.Find("Player_Cell");
        eMain = GameObject.Find("Enemy_Cell");
        idleState = new ECIdleState(gameObject,this);
        defendState = new ECDefendState(gameObject,this);
        avoidState = new ECAvoidState(gameObject,this);
        attackState = new ECAttackState(gameObject,this);
        chargeCState = new ECChargeCState(gameObject,this);
        chargeMState = new ECChargeMState(gameObject,this);
        tAttackState = new ECTrickAttackState(gameObject,this);
        deadState = new ECDeadState(gameObject,this);
        mineState = new ECMineState(gameObject,this);
        currentState = deadState;
        currentCommand = MessageType.Idle;
        type = GetType();
    }

    void Update()
    {
        currentState.Execute();
        Debug.Log(currentCommand.ToString());
        UpdateState();
    }

    public int Index
    {
        get { return nIndex; }
        set { nIndex = value; }
    }

    public ECStatus Type
    {
        get { return type; }
        set { type = value; }
    }

    public MessageType Command
    {
        get { return currentCommand; }
        set { currentCommand = value; }
    }

    public IECState CurrentState
    {
        get { return currentState; }
    }

    public void ChangeState(IECState state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
        currentCommand = MessageType.Empty;
    }

    private void UpdateState()
    {
        if (currentCommand == MessageType.Avoid)
        {
            ChangeState(avoidState);
        }
        else if (currentCommand == MessageType.Attack)
        {
            ChangeState(attackState);
        }
        else if (currentCommand == MessageType.Dead)
        {
            ChangeState(deadState);
        }
        else if (currentCommand == MessageType.Defend)
        {
            ChangeState(defendState);
        }
        else if (currentCommand == MessageType.Idle)
        {
            ChangeState(idleState);
        }
        else if (currentCommand == MessageType.Landmine)
        {
            ChangeState(mineState);
        }
    }

    public int AmountOfSameCellState()
    {
        GameObject[] childCells = GameObject.FindGameObjectsWithTag("EnemyChild");
        int count = 0;
        for (int i = 0; i < childCells.Length; i++)
        {
            if (currentState == childCells[i].GetComponent<EnemyChildFSM>().currentState)
            {
                count++;
            }
        }
        return count;
    }

    private ECStatus GetType()
    {
        List<GameObject> childList = eMain.GetComponent<EnemyMainFSM>().ecList;
        for (int i = 0; i < childList.Count; i++)
        {
            if (childList[i].GetComponent<EnemyChildFSM>().Type == ECStatus.Leader)
            {
                return ECStatus.Member;
            }
        }
        return ECStatus.Leader;
    }

    /*private bool IsUnderAttack()
    {

    }*/
}
