using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECIdleState : IECState
{
 
    public ECIdleState(GameObject childCell, EnemyChildFSM ecFSM)
    {
        child = childCell;
        m_ecFSM = ecFSM;

    }

    public override void Enter()
    {

    }

    public override void Execute()
    {
        child.GetComponent<Rigidbody2D>().velocity = Seperation();

        /*Vector2 velocity = new Vector2(Alignment().x + Cohesion().x + Seperation().x, Alignment().y + Cohesion().y + Seperation().y);
        velocity.Normalize();
        child.GetComponent<Rigidbody2D>().velocity = velocity * m_ecFSM.fSpeed;*/
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
}