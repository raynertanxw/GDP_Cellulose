using UnityEngine;
using System.Collections;

public class ECAttackState : IECState {

    public ECAttackState(GameObject childCell, EnemyChildFSM ecFSM)
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
