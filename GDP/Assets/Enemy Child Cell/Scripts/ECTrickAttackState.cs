using UnityEngine;
using System.Collections;

public class ECTrickAttackState : IECState {

    public ECTrickAttackState(GameObject childCell, EnemyChildFSM ecFSM)
    {
        child = childCell;
        m_ecFSM = ecFSM;
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
}
