﻿using UnityEngine;
using System.Collections;

public class ECDefendState : IECState {

    private Vector2 m_TargetPos;
    private GameObject m_EnemyMain;

    public ECDefendState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_EnemyMain = m_ecFSM.eMain.gameObject;
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

    /* private Vector2 FindDefendPos()
     {
        float noise = Random.Range(0.1f, 0.6f);
        float range = EnemyMain.GetComponent<SpriteRenderer>().bounds.size.y + 2f * gameObject.GetComponent<SpriteRenderer>().bounds.size.y + noise;
        

    }

    private Vector2 IdentifyAttackDirection()
    {
        GameObject[] playerChild = GameObject.FindGameObjectsWithTag("PlayerChild");
        for (int i = 0; i < playerChild.Length; i++)
        {
            if(playerChild[i].GetComponent<PlayerChildFSM>().)
        }
    }*/

    /* private bool IsPosOccupied()
     { 

     }

     private void MoveTowards(Vector2 pos)
     { 

     }

     private bool IsEnemyInRange()
     { 

     }*/
}
