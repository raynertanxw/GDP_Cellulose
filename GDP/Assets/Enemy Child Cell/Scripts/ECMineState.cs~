using UnityEngine;
using System.Collections;

public class ECMineState : IECState {

    private Vector2 m_MinePos;
    private float fMineSpeed;

    public ECMineState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
        fMineSpeed = 0.6f * m_ecFSM.fSpeed;
    }

    public override void Enter()
    {
		m_MinePos = FindMinePos();
		while (IsPosOccupied(m_MinePos) == true)
        {
            FindMinePos();
        }
    }

    public override void Execute()
    {
		if (HasCellReachTargetPos(m_MinePos) == false)
        {
			MoveTowards(m_MinePos);
        }

        if (IsPCNearby())
        {
            Explode();
			MessageDispatcher.Instance.DispatchMessage(m_Child, m_ecFSM.gameObject, MessageType.Dead, 0);
        }
    }

    public override void Exit()
    {

    }

    private Vector2 FindMinePos()
    {
        GameObject LeftWall = GameObject.Find("Left Wall");
        GameObject RightWall = GameObject.Find("Right Wall");

		float PosX = Random.Range(LeftWall.transform.position.x + LeftWall.GetComponent<SpriteRenderer>().bounds.size.x / 2 + m_Child.GetComponent<SpriteRenderer>().bounds.size.x / 2, RightWall.transform.position.x - RightWall.GetComponent<SpriteRenderer>().bounds.size.x / 2 - m_Child.GetComponent<SpriteRenderer>().bounds.size.x / 2);
		float PosY = Random.Range((m_Child.transform.position.y + m_ecFSM.pMain.transform.position.y) / 2, m_Child.transform.position.y);

        Vector2 targetPos = new Vector2(PosX, PosY);
        return targetPos;
    }

    private bool IsPosOccupied(Vector2 _Pos)
    {
		if (Physics2D.OverlapCircle(_Pos, m_Child.GetComponent<SpriteRenderer>().bounds.size.x) != null)
        {
            return true;
        }
        return false;
    }

	private void MoveTowards(Vector2 _Pos)
    {
		Vector2 difference = new Vector2(_Pos.x - m_Child.transform.position.x, _Pos.y - m_Child.transform.position.y);
        Vector2 direction = difference.normalized;

		m_Child.transform.Translate(direction * fMineSpeed);
    }
	private bool HasCellReachTargetPos(Vector2 _Pos)
    {
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= 0.1f)
        {
            return true;
        }
        return false;
    }

    private bool IsPCNearby()
    {
		Collider2D[] ObjectsAroundChild = Physics2D.OverlapCircleAll(m_Child.transform.position, 1.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
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
		Collider2D[] ObjectsAroundChild = Physics2D.OverlapCircleAll(m_Child.transform.position, 1.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
        for (int i = 0; i < ObjectsAroundChild.Length; i++)
        {
            if (ObjectsAroundChild[i].gameObject.tag == "PlayerChild")
            {
                //kill the player child cell
            }
        }
    }
}
