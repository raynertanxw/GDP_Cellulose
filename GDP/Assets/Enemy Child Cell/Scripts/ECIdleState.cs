using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECIdleState : IECState
{
    private Vector2 targetPos;
    private float timer;
    private float height;
    private bool foundInitialPos;

    public ECIdleState(GameObject childCell, EnemyChildFSM ecFSM)
    {
        child = childCell;
        m_ecFSM = ecFSM;
    }

    public override void Enter()
    {
        timer = 0.0f;
        
        foundInitialPos = false;

        targetPos = GenerateInitialPos();
        Debug.Log(targetPos);
    }

    public override void Execute()
    {
        Debug.Log(timer);
        Debug.Log(foundInitialPos);

        if (!foundInitialPos)
        {
            MoveTowards(targetPos);
            if (HasCellReachTargetPos(targetPos))
            {
                foundInitialPos = true;
            }
        }

        if (timer <= 4f)
        {
            Seperation();
        }

        if (timer > 4f && timer <= 8f)
        {
            Cohesion();
        }

        if (timer > 8f)
        {
            timer = 0.0f;
            height = GenerateYPoint();
        }

        timer += Time.deltaTime;
    }

    public override void Exit()
    {

    }

    private float GenerateYPoint()
    {
        float minY = -m_ecFSM.eMain.GetComponent<SpriteRenderer>().bounds.size.y / 2 - (3 * child.GetComponent<SpriteRenderer>().bounds.size.y / 2);
        float maxY = m_ecFSM.eMain.GetComponent<SpriteRenderer>().bounds.size.y / 2 + (3 * child.GetComponent<SpriteRenderer>().bounds.size.y / 2);
        return Random.Range(minY, maxY);
    }

    private Vector2 GenerateInitialPos()
    {
        float maxX = GameObject.Find("Right Wall").transform.position.x - GameObject.Find("Right Wall").GetComponent<SpriteRenderer>().bounds.size.x/2 - child.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        float minX = GameObject.Find("Left Wall").transform.position.x + GameObject.Find("Left Wall").GetComponent<SpriteRenderer>().bounds.size.x / 2 + child.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        float minY = -m_ecFSM.eMain.GetComponent<SpriteRenderer>().bounds.size.y / 2 - (3 * child.GetComponent<SpriteRenderer>().bounds.size.y / 2);
        float maxY = m_ecFSM.eMain.GetComponent<SpriteRenderer>().bounds.size.y / 2 + (3 * child.GetComponent<SpriteRenderer>().bounds.size.y / 2);
        float Y = m_ecFSM.eMain.transform.position.y + Random.Range(minY, maxY);
        return new Vector2(Random.Range(minX, maxX), Y);
    }

    private void ProjectPosToMain(Vector2 pos)
    {
        if (!HasCellReachTargetPos(new Vector2(child.transform.position.x, height)))
        {
            child.transform.Translate(Vector2.up * m_ecFSM.fSpeed);
        }
    }

    private void Seperation()
    {
        Vector2 steering = new Vector2(0f, 0f);
        Vector2 diff = new Vector2(m_ecFSM.eMain.transform.position.x - child.transform.position.x, m_ecFSM.eMain.transform.position.y - child.transform.position.y);
        steering = -diff.normalized;
        child.transform.Translate(steering * m_ecFSM.fSpeed);
        ProjectPosToMain(child.transform.position);
    }

    private void Cohesion()
    {
        Vector2 steering = new Vector2(0f, 0f);
        Vector2 diff = new Vector2(m_ecFSM.eMain.transform.position.x - child.transform.position.x, m_ecFSM.eMain.transform.position.y - child.transform.position.y);
        steering = diff.normalized;
        child.transform.Translate(steering * m_ecFSM.fSpeed);
        ProjectPosToMain(child.transform.position);
    }

    private bool IsWithinIdleRange()
    {
        if (Vector2.Distance(child.transform.position, m_ecFSM.eMain.transform.position) < (m_ecFSM.eMain.GetComponent<SpriteRenderer>().bounds.size.x / 2 + (3 * child.GetComponent<SpriteRenderer>().bounds.size.x / 2)))
        {
            return true;
        }
        return false;
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