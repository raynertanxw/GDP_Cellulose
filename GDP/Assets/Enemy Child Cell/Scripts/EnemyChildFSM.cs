using UnityEngine;
using System.Collections;

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

    private MessageType currentCommand;

    void Start()
    {
        fSpeed = 0.5f;
        bIsMine = false;
        pMain = GameObject.Find("Player_Cell");
        eMain = GameObject.Find("Enemy_Cell");
        idleState = new ECIdleState(this);
        defendState = new ECDefendState(this);
        avoidState = new ECAvoidState(this);
        attackState = new ECAttackState(this);
        chargeCState = new ECChargeCState(this);
        chargeMState = new ECChargeMState(this);
        tAttackState = new ECTrickAttackState(this);
        deadState = new ECDeadState(this);
        mineState = new ECMineState(this);
        currentState = deadState;
        currentCommand = MessageType.Empty;
    }

    void Update()
    {
        currentState.Execute();
        UpdateState();
    }

    public int Index
    {
        get { return nIndex; }
        set { nIndex = value; }
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
        else if (currentCommand == MessageType.ChargeChild)
        {
            ChangeState(chargeCState);
        }
        else if (currentCommand == MessageType.ChargeMain)
        {
            ChangeState(chargeMState);
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

    /*private bool IsUnderAttack()
    {

    }*/
}
