﻿using UnityEngine;
using System.Collections;

public class ECDeadState : IECState {

    public ECDeadState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
    }

    public override void Enter()
    {
		m_Child.GetComponent<SpriteRenderer>().enabled = false;
		m_Child.GetComponent<BoxCollider2D>().enabled = false;
		m_Child.GetComponent<Rigidbody2D>().isKinematic = true;
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
		m_Child.GetComponent<SpriteRenderer>().enabled = true;
		m_Child.GetComponent<BoxCollider2D>().enabled = true;
		m_Child.GetComponent<Rigidbody2D>().isKinematic = false;
    }
}
