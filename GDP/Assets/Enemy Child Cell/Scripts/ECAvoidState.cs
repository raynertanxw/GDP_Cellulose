﻿using UnityEngine;
using System.Collections;

public class ECAvoidState : IECState {

    // Use this for initialization
    public ECAvoidState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
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

    /*private Vector2 FindTargetPos()
    {
    
    }

    private void MoveTowards(Vector2 pos)
    {
    
    }

    private List<GameObject> ReturnAttackers()
    {
    
    }

    private int ReturnEMHealth()
    {
    
    }

    private bool IsPCNearby()
    {
    
    }*/
}
