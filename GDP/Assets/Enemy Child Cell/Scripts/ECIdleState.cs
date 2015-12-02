using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECIdleState : IECState
{
    private float alignStrength;
    private float cohesionStrength;
    private float seperateStrength;
    private float wanderStrength;
    private float wanderRadius;
    private float wanderDistance;
    private float wanderJitter;
    private float timer;
    private bool IsWondering;
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
        wanderStrength = 0.0f;
        wanderRadius = 2.5f * child.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        wanderDistance = 2f * child.GetComponent<SpriteRenderer>().bounds.size.x;
        wanderJitter = 2f;
        timer = 0.0f;
        reachInitialPos = false;

        initialTarget = GenerateRandomPos();
        IsWondering = true;
        m_ecFSM.StartChildCorountine(Wandering());

    }

    public override void Execute()
    {
        if (IsWondering == false && (HittingWall(child.transform.position) || LeavingIdleRange(child.transform.position)))
        {
            child.GetComponent<Rigidbody2D>().velocity = GenerateInverseVelo(child.GetComponent<Rigidbody2D>().velocity);
            Debug.Log("change in execute");
        }

        /*in progress
        
        //if this child cell is the only child cell in the main cell it is not wondering now, start the wandering process
        if (main.GetComponent<EnemyMainFSM>().ECList.Count <= 1 && IsWondering == false)
        {
            IsWondering = true;
            m_ecFSM.StartChildCorountine(Wandering());
        }
        else if (main.GetComponent<EnemyMainFSM>().ECList.Count > 1)
        {
            if (IsWondering == true)
            {
                IsWondering = false;
            }



            float veloX = Alignment().x * alignStrength + Cohesion().x * cohesionStrength + Seperation().x * seperateStrength + Wander().x * wanderStrength;
            float veloY = Alignment().y * alignStrength + Cohesion().y * cohesionStrength + Seperation().y * seperateStrength + Wander().y * wanderStrength;

            Vector2 velocity = new Vector2(veloX, veloY);
            child.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y + m_ecFSM.eMain.GetComponent<Rigidbody2D>().velocity.y);
        }

        //if the child cell is not wandering but it is going to leave the idle range or hit the wall, reverse its velocity
        if (IsWondering == false && (HittingWall(child.transform.position) || LeavingIdleRange(child.transform.position)))
        {
            child.GetComponent<Rigidbody2D>().velocity = GenerateInverseVelo(child.GetComponent<Rigidbody2D>().velocity);
        }
        }*/
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

    private Vector2 GenerateInverseVelo(Vector2 velo)
    {
        //Debug.Log("current velo: " + child.GetComponent<Rigidbody2D>().velocity);

        Vector2 inverse = velo;

        Vector2 diff = new Vector2(child.transform.position.x - main.transform.position.x, child.transform.position.y - main.transform.position.y);
        inverse = -diff.normalized;

        return inverse;

        /* if (velo.x > velo.y)
         {
             inverse.x = -inverse.x;
             int sign = Random.Range(-1, 1);
             if (sign == 0)
             {
                 sign = -1;
             }
             inverse.y *= sign;
             if (inverse.y < 0)
             {
                 inverse.y -= 0.3f;
             }
             else if (inverse.y > 0)
             {
                 inverse.y += 0.5f;
             }
         }
         else if (velo.y > velo.x)
         {
             inverse.y = -inverse.y;
             int sign = Random.Range(-1, 1);
             if (sign == 0)
             {
                 sign = -1;
             }
             inverse.x *= sign;
         }
         return inverse;*/
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 targetPos = child.transform.position;
        Vector2 difference = new Vector2(target.x - child.transform.position.x, target.y - child.transform.position.y);
        Vector2 direction = difference.normalized;
        Vector2 towards = new Vector2(child.transform.position.x + direction.x * m_ecFSM.fSpeed, child.transform.position.y + direction.y * m_ecFSM.fSpeed);
        child.GetComponent<Rigidbody2D>().MovePosition(towards);
    }

    private bool HittingWall(Vector2 pos)
    {
        if (Vector2.Distance(pos, GameObject.Find("Right Wall").transform.position) < child.GetComponent<SpriteRenderer>().bounds.size.x || Vector2.Distance(pos, GameObject.Find("Left Wall").transform.position) < child.GetComponent<SpriteRenderer>().bounds.size.x)
        {
            return true;
        }
        return false;
    }

    private bool LeavingIdleRange(Vector2 pos)
    {
        float maxY = main.transform.position.y + main.GetComponent<SpriteRenderer>().bounds.size.x / 2 + 3.5f * child.GetComponent<SpriteRenderer>().bounds.size.x;
        float minY = main.transform.position.y - main.GetComponent<SpriteRenderer>().bounds.size.x / 2 - 3.5f * child.GetComponent<SpriteRenderer>().bounds.size.x;
        float maxX = main.transform.position.x + main.GetComponent<SpriteRenderer>().bounds.size.x / 2 + 5f * child.GetComponent<SpriteRenderer>().bounds.size.x;
        float minX = main.transform.position.x - main.GetComponent<SpriteRenderer>().bounds.size.x / 2 - 5f * child.GetComponent<SpriteRenderer>().bounds.size.x;

        if (pos.y > maxY || pos.y < minY || pos.x > maxX || pos.x < minX)
        {
            return true;
        }
        return false;
    }

    private Vector2 ReturnToIdleRange()
    {
        Vector2 target = GenerateRandomPos();
        Vector2 diff = new Vector2(target.x - child.transform.position.x, target.y - child.transform.position.y);
        diff.Normalize();
        return new Vector2(diff.x, diff.y + main.GetComponent<Rigidbody2D>().velocity.y);
    }

    public IEnumerator Wandering()
    {
        bool Recover = false;

        while (IsWondering == true && cohesionStrength == 0.0f && seperateStrength == 0.0f && alignStrength == 0.0f)
        {
            if (HittingWall(child.transform.position) || LeavingIdleRange(child.transform.position))
            {
                /*Vector2 current = child.GetComponent<Rigidbody2D>().velocity;
                Vector2 inverse = GenerateInverseVelo(current);
                child.GetComponent<Rigidbody2D>().velocity = inverse;*/

                Recover = true;

                /*float velocity = inverse.magnitude;
                float distance = Vector2.Distance(child.transform.position, main.transform.position);
                float initialTime = distance / velocity;

                float mVelo = main.GetComponent<Rigidbody2D>().velocity.magnitude;
                float mDistance = mVelo * initialTime;

                float predictY = Mathf.Pow(mDistance, 2f) + main.transform.position.y;
                Vector2 predictedMPos = new Vector2(main.transform.position.x, predictY);

                float travelToPredict = Vector2.Distance(child.transform.position, predictedMPos) / child.GetComponent<Rigidbody2D>().velocity.magnitude;

                float duration = Random.Range(travelToPredict - 0.5f, travelToPredict);
                yield return new WaitForSeconds(duration);*/
            }

            if (Recover == true)
            {
                if (Vector2.Distance(child.transform.position, main.transform.position) <= child.GetComponent<SpriteRenderer>().bounds.size.x + main.GetComponent<SpriteRenderer>().bounds.size.x / 2)
                {
                    Recover = false;
                }

                Vector2 current = child.GetComponent<Rigidbody2D>().velocity;
                Vector2 inverse = GenerateInverseVelo(current);
                inverse.y *= 1.2f;
                child.GetComponent<Rigidbody2D>().velocity = inverse;
                yield return new WaitForSeconds(0.1f);
            }
            else if (Recover == false)
            {
                Vector2 velo = Wander();
                velo.y += m_ecFSM.eMain.GetComponent<Rigidbody2D>().velocity.y;
                child.GetComponent<Rigidbody2D>().velocity = velo;
                float duration = Random.Range(0.5f, 1f);
                yield return new WaitForSeconds(duration);
            }

        }
        IsWondering = false;
        yield break;
    }

    private Vector2 Wander()
    {
        Vector2 target = new Vector2(child.transform.position.x + Random.Range(-6f, 6f), child.transform.position.y + Random.Range(-1f, 1f));
        Vector2 projection = target.normalized * wanderRadius;
        projection += new Vector2(0f, wanderDistance);
        projection *= Random.Range(-1f, 1f);
        projection.x += Random.Range(-0.3f, 0.3f);
        return projection.normalized;
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