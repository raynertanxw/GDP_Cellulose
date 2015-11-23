using UnityEngine;
using System.Collections;

public class ECChargeCState : IECState {

    private float fChargeSpeed;
    public GameObject target;

    public ECChargeCState(GameObject childCell, EnemyChildFSM ecFSM)
    {
        m_ecFSM = ecFSM;
        child = childCell;
        fChargeSpeed = 0.3f;
    }

    public override void Enter()
    {
        target = FindTargetChild();
    }

    public override void Execute()
    {
        if (HasCellReachTargetPos(target.transform.position) == false)
        {
            ChargeTowards(target);
        }
        else
        {
            MessageDispatcher.Instance.DispatchMessage(child, m_ecFSM.gameObject, MessageType.Dead, 0.0);
        }
    }

    public override void Exit()
    {
    }

    private bool CheckIfTargetIsAvailable(GameObject GO)
    {
        GameObject[] childCells = GameObject.FindGameObjectsWithTag("EnemyChild");
        for (int i = 0; i < childCells.Length; i++)
        {
            if (childCells[i].GetComponent<ECChargeCState>().target == GO)
            {
                return false;
            }
        }
        return true;
    }


    private GameObject FindTargetChild()
    {
        GameObject[] childCells = GameObject.FindGameObjectsWithTag("PlayerChild");
        float distanceBetween = Mathf.Infinity;
        GameObject closestChild = childCells[0];

        for (int i = 0; i < childCells.Length; i++)
        {
            if (CheckIfTargetIsAvailable(childCells[i]) && (Vector2.Distance(child.transform.position, childCells[i].transform.position)) < distanceBetween)
            {
                distanceBetween = Vector2.Distance(child.transform.position, childCells[i].transform.position);
                closestChild = childCells[i];
            }
        }

        return closestChild;
    }


    private bool HasCellReachTargetPos(Vector2 pos)
    {
        if (Vector2.Distance(child.transform.position, pos) <= 0.1f)
        {
            return true;
        }
        return false;
    }

    private void ChargeTowards(GameObject PC)
    {
        Vector2 targetPos = PC.transform.position;
        Vector2 difference = new Vector2(targetPos.x - child.transform.position.x, targetPos.y - child.transform.position.y);
        Vector2 direction = difference.normalized;

        child.transform.Translate(direction * m_ecFSM.fSpeed);
    }
}
