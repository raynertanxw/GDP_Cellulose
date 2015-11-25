using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECIdleState : IECState
{
    private float alignStrength;
    private float cohesionStrength;
    private float seperateStrength;
    private float timer;
    private bool reachInitialPos;
    private GameObject main;
    private Vector2 initialTarget;

    public ECIdleState(GameObject childCell, EnemyChildFSM ecFSM)
    {
        child = childCell;
        m_ecFSM = ecFSM;
        main = m_ecFSM.eMain;
    }

    public override void Enter()
    {
        alignStrength = 0.0f;
        cohesionStrength = 0.0f;
        seperateStrength = 0.0f;
        timer = 0.0f;
        reachInitialPos = false;

        initialTarget = GenerateRandomPos();
    }

    public override void Execute()
    {
        if (!reachInitialPos)
        {
            MoveTowards(initialTarget);
            if (HasCellReachPosition(initialTarget))
            {
                reachInitialPos = true;
            }
        }
        else
        {
            if (timer < 3f)
            {
                seperateStrength = 0.0f;
                cohesionStrength = 1.0f;
            }
            else if(timer > 6f)
            {
                seperateStrength = 1.0f;
                cohesionStrength = 0.0f;
                timer = 0.0f;
            }

            Vector2 velocity = new Vector2(Alignment().x * alignStrength + Cohesion().x * cohesionStrength + Seperation().x * seperateStrength, Alignment().y * alignStrength + Cohesion().y * cohesionStrength + Seperation().y * seperateStrength);
            child.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y + m_ecFSM.eMain.GetComponent<Rigidbody2D>().velocity.y);

            timer += Time.deltaTime;
        }
    }

    public override void Exit()
    {

    }

    private GameObject[] TagNeighbours()
    {
        Collider2D[] GOaround = Physics2D.OverlapCircleAll(child.transform.position, 5 * child.GetComponent<SpriteRenderer>().bounds.size.x);
        GameObject[] neighbours = new GameObject[GOaround.Length];
        int count = 0;
        for (int i = 0; i < GOaround.Length; i++)
        {
            if (GOaround[i].gameObject.tag == "EnemyChild")
            {
                neighbours[count] = GOaround[i].gameObject;
                count++;
            }
        }
        return neighbours;
    }

    private Vector2 GenerateRandomPos()
    {
        float minX = GameObject.Find("Left Wall").transform.position.x + GameObject.Find("Left Wall").GetComponent<SpriteRenderer>().bounds.size.x / 2 + child.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        float maxX = GameObject.Find("Right Wall").transform.position.x - GameObject.Find("Right Wall").GetComponent<SpriteRenderer>().bounds.size.x / 2 - child.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        float minY = main.transform.position.y - 1.5f * main.GetComponent<SpriteRenderer>().bounds.size.x;
        float maxY = main.transform.position.y + 1.5f * main.GetComponent<SpriteRenderer>().bounds.size.x;

        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 targetPos = child.transform.position;
        Vector2 difference = new Vector2(target.x - child.transform.position.x, target.y - child.transform.position.y);
        Vector2 direction = difference.normalized;
        Vector2 towards = new Vector2(child.transform.position.x + direction.x * m_ecFSM.fSpeed, child.transform.position.y + direction.y * m_ecFSM.fSpeed);
        child.GetComponent<Rigidbody2D>().MovePosition(towards);
    }

    private Vector2 Alignment()
    {
        GameObject[] neighbours = TagNeighbours();
        int neighbourCount = 0;
        Vector2 steering = new Vector2(0f, 0f);

        foreach (GameObject cell in neighbours)
        {
            if (cell != null && cell != child)
            {
                steering.x += cell.GetComponent<Rigidbody2D>().velocity.x;
                steering.y += cell.GetComponent<Rigidbody2D>().velocity.y;
                neighbourCount++;
            }
        }

        if (neighbourCount <= 0)
        {
            return steering;
        }
        else
        {
            steering /= neighbourCount;
            steering.Normalize();
            return steering;
        }
    }

    private Vector2 Seperation()
    {
        GameObject[] neighbours = TagNeighbours();
        int neighbourCount = 0;
        Vector2 steering = new Vector2(0f, 0f);

        foreach (GameObject cell in neighbours)
        {
            if (cell != null && cell != child)
            {
                steering.x += cell.transform.position.x - child.transform.position.x;
                steering.y += cell.transform.position.y - child.transform.position.y;
                neighbourCount++;
            }
        }

        if (neighbourCount <= 0)
        {
            return steering;
        }
        else
        {
            steering /= neighbourCount;
            steering *= -1f;
            steering.Normalize();
            return steering;
        }
    }

    private Vector2 Cohesion()
    {
        GameObject[] neighbours = TagNeighbours();
        int neighbourCount = 0;
        Vector2 steering = new Vector2(0f, 0f);

        foreach (GameObject cell in neighbours)
        {
            if (cell != null && cell != child)
            {
                steering.x += cell.transform.position.x;
                steering.y += cell.transform.position.y;
                neighbourCount++;
            }
        }

        if (neighbourCount <= 0)
        {
            return steering;
        }
        else
        {
            steering /= neighbourCount;
            steering = new Vector2(steering.x - child.transform.position.x, steering.y - child.transform.position.y);
            steering.Normalize();
            return steering;
        }
    }

    private bool HasCellReachPosition(Vector2 pos)
    {
        if (Vector2.Distance(child.transform.position, pos) < 0.01f)
        {
            return true;
        }
        return false;
    }
}