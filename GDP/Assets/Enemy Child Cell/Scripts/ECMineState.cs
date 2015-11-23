using UnityEngine;
using System.Collections;

public class ECMineState : IECState {

    private Vector2 minePos;
    private float fMineSpeed;

    public ECMineState(GameObject childCell, EnemyChildFSM ecFSM)
    {
        child = childCell;
        m_ecFSM = ecFSM;
        fMineSpeed = 0.6f * m_ecFSM.fSpeed;
    }

    public override void Enter()
    {
        minePos = FindMinePos();
        while (IsPosOccupied(minePos) == true)
        {
            FindMinePos();
        }
    }

    public override void Execute()
    {
        if (HasCellReachTargetPos(minePos) == false)
        {
            MoveTowards(minePos);
        }

        if (IsPCNearby())
        {
            Explode();
            MessageDispatcher.Instance.DispatchMessage(child, m_ecFSM.gameObject, MessageType.Dead, 0);
        }
    }

    public override void Exit()
    {

    }

    private Vector2 FindMinePos()
    {
        GameObject LeftWall = GameObject.Find("Left Wall");
        GameObject RightWall = GameObject.Find("Right Wall");

        float PosX = Random.Range(LeftWall.transform.position.x + LeftWall.GetComponent<SpriteRenderer>().bounds.size.x / 2 + child.GetComponent<SpriteRenderer>().bounds.size.x / 2, RightWall.transform.position.x - RightWall.GetComponent<SpriteRenderer>().bounds.size.x / 2 - child.GetComponent<SpriteRenderer>().bounds.size.x / 2);
        float PosY = Random.Range((child.transform.position.y + m_ecFSM.pMain.transform.position.y) / 2, child.transform.position.y);

        Vector2 targetPos = new Vector2(PosX, PosY);
        return targetPos;
    }

    private bool IsPosOccupied(Vector2 pos)
    {
        if (Physics2D.OverlapCircle(pos, child.GetComponent<SpriteRenderer>().bounds.size.x) != null)
        {
            return true;
        }
        return false;
    }

    private void MoveTowards(Vector2 pos)
    {
        Vector2 difference = new Vector2(pos.x - child.transform.position.x, pos.y - child.transform.position.y);
        Vector2 direction = difference.normalized;

        child.transform.Translate(direction * fMineSpeed);
    }
    private bool HasCellReachTargetPos(Vector2 pos)
    {
        if (Vector2.Distance(child.transform.position, pos) <= 0.1f)
        {
            return true;
        }
        return false;
    }

    private bool IsPCNearby()
    {
        Collider2D[] ObjectsAroundChild = Physics2D.OverlapCircleAll(child.transform.position, 1.5f * child.GetComponent<SpriteRenderer>().bounds.size.x);
        for (int i = 0; i < ObjectsAroundChild.Length; i++)
        {
            if (ObjectsAroundChild[i].gameObject.tag == "PlayerChild")
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(3f);
        Collider2D[] ObjectsAroundChild = Physics2D.OverlapCircleAll(child.transform.position, 1.5f * child.GetComponent<SpriteRenderer>().bounds.size.x);
        for (int i = 0; i < ObjectsAroundChild.Length; i++)
        {
            if (ObjectsAroundChild[i].gameObject.tag == "PlayerChild")
            {
                //kill the player child cell
            }
        }
    }
}
