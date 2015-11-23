using UnityEngine;
using System.Collections;

public class ECDeadState : IECState {

    public ECDeadState(GameObject childCell, EnemyChildFSM ecFSM)
    {
        child = childCell;
        m_ecFSM = ecFSM;
    }

    public override void Enter()
    {
        child.GetComponent<SpriteRenderer>().enabled = false;
        child.GetComponent<BoxCollider2D>().enabled = false;
        child.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        Reactivate();
    }

    private void Reactivate()
    {
        child.GetComponent<SpriteRenderer>().enabled = true;
        child.GetComponent<BoxCollider2D>().enabled = true;
        child.GetComponent<Rigidbody2D>().isKinematic = false;
    }
}
