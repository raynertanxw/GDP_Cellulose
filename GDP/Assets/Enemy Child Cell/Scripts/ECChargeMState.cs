using UnityEngine;
using System.Collections;

public class ECChargeMState : IECState {

    private GameObject PM;

    public ECChargeMState(GameObject childCell, EnemyChildFSM ecFSM)
    {
        child = childCell;
        m_ecFSM = ecFSM;
        PM = m_ecFSM.pMain;
    }

    public override void Enter()
    {

    }

    public override void Execute()
    {
        if (HasCellReachTargetPos(PM.transform.position) == false)
        {
            ChargeTowards(PM);
        }
        else
        {
            MessageDispatcher.Instance.DispatchMessage(child, m_ecFSM.gameObject, MessageType.Dead, 0.0);
        }
    }

    public override void Exit()
    {

    }

    private bool HasCellReachTargetPos(Vector2 pos)
    {
        if (Vector2.Distance(child.transform.position, pos) <= 0.1f)
        {
            return true;
        }
        return false;
    }

    private void ChargeTowards(GameObject PM)
    {
        Vector2 targetPos = PM.transform.position;
        Vector2 difference = new Vector2(targetPos.x - child.transform.position.x, targetPos.y - child.transform.position.y);
        Vector2 direction = difference.normalized;

        child.transform.Translate(direction * m_ecFSM.fSpeed);
    }
}

