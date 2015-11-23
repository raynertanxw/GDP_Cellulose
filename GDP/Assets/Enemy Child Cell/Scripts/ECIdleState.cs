﻿using UnityEngine;
using System.Collections;

public class ECIdleState : IECState
{

    private float fWanderRadius;//radius of constraining circle
    private float fProjectDistance;//distance that the circle is projected in front of the agent
    private float fWanderJitter;//max amount of displacement for the target position each timestep 
    private Vector2 WanderTarget;
    private Vector2 PreviousKnownMainPos;

    public ECIdleState(GameObject childCell, EnemyChildFSM ecFSM)
    {
        child = childCell;
        m_ecFSM = ecFSM;
        fWanderRadius = 2f;
        fProjectDistance = 3 * (child.GetComponent<SpriteRenderer>().bounds.size.x / 2);
        fWanderJitter = 0.5f;
        WanderTarget = new Vector2(0, 0);
    }

    public override void Enter()
    {
        if (IsWithinIdleRange() == false)
        {
            TravelToIdleArea();
        }
        WanderTarget = child.transform.position;
        WanderTarget = Wander();
        PreviousKnownMainPos = m_ecFSM.eMain.transform.position;
    }

    public override void Execute()
    {
        if (HasCellReachTargetPos(WanderTarget) == false)
        {
            MoveTowards(WanderTarget);
            Debug.Log("Move towards: " + WanderTarget);
        }
        else
        {
            WanderTarget = Wander();
            Debug.Log("reset");
        }

        //add an if statement to check if the enemy main cell had received heavy damage
        //if it is, change the child state to defend
    }

    public override void Exit()
    {

    }

    private bool IsWithinIdleRange()
    {
        if (Vector2.Distance(child.transform.position, m_ecFSM.eMain.transform.position) < (m_ecFSM.eMain.GetComponent<SpriteRenderer>().bounds.size.x / 2 + (3 * child.GetComponent<SpriteRenderer>().bounds.size.x / 2)))
        {
            return true;
        }
        return false;
    }

    public void TravelToIdleArea()
    {
        Vector2 currentPos = child.transform.position;
        Vector2 heading = new Vector2(child.transform.position.x - m_ecFSM.eMain.transform.position.x, child.transform.position.y - m_ecFSM.eMain.transform.position.y);
        Vector2 direction = heading.normalized;
        float fDistanceDiff = Vector2.Distance(child.transform.position, m_ecFSM.eMain.transform.position) - (m_ecFSM.eMain.GetComponent<SpriteRenderer>().bounds.size.x / 2 + (3 * child.GetComponent<SpriteRenderer>().bounds.size.x / 2));
        float fDistanceTraveled = 0f;
        while (fDistanceTraveled < fDistanceDiff)
        {
            Vector2 previousPos = child.transform.position;
            child.transform.Translate(direction * m_ecFSM.fSpeed);
            Vector2 nextPos = child.transform.position;
            fDistanceTraveled += Vector2.Distance(previousPos, nextPos);
            Debug.Log("Travel to idle Area");
        }
    }

    private Vector2 Wander()
    {
        WanderTarget += new Vector2(Random.Range(-0.5f, 0.5f) * fWanderJitter, Random.Range(-0.5f, 0.5f) * fWanderJitter);
        if (PreviousKnownMainPos.y != m_ecFSM.eMain.transform.position.y)
        {
            float diffY = m_ecFSM.eMain.transform.position.y - child.transform.position.y;
            WanderTarget.y += diffY;
        }
        /* Debug.Log("previous: " + PreviousKnownMainPos.y + "   eMain: " + m_ecFSM.eMain.transform.position.y);


         WanderTarget.Normalize();
         WanderTarget *= fWanderRadius;

         Vector2 targetLocal = WanderTarget + new Vector2(fProjectDistance, fProjectDistance);
         if (PreviousKnownMainPos.y != m_ecFSM.eMain.transform.position.y)
         {
             float diffY = m_ecFSM.eMain.transform.position.y - child.transform.position.y;
             targetLocal.y += diffY;            
             // WanderTarget.y += diffY;
         }*/

        return WanderTarget;
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 diff = new Vector2(target.x - child.transform.position.x, target.y - child.transform.position.y);
        Vector2 direction = diff.normalized;
        child.transform.Translate(direction * m_ecFSM.fSpeed);
    }

    private bool HasCellReachTargetPos(Vector2 pos)
    {
        if (Vector2.Distance(child.transform.position, pos) <= 0.1f)
        {
            return true;
        }
        return false;
    }

    /*private int ReturnEMHealth()
    {
        return m_ecFSM.eMain.nDamageNum;
    }*/
}