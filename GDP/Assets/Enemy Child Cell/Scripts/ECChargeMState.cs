﻿using UnityEngine;
using System.Collections;

public class ECChargeMState : IECState {

    private GameObject m_PlayerMain;

    public ECChargeMState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_PlayerMain = m_ecFSM.pMain;
    }

    public override void Enter()
    {

    }

    public override void Execute()
    {
		if (HasCellReachTargetPos(m_PlayerMain.transform.position) == false)
        {
			ChargeTowards(m_PlayerMain);
        }
        else
        {
            MessageDispatcher.Instance.DispatchMessage(m_Child, m_ecFSM.gameObject, MessageType.Dead, 0.0);
        }
    }

    public override void Exit()
    {

    }

    private bool HasCellReachTargetPos(Vector2 pos)
    {
		if (Vector2.Distance(m_Child.transform.position, pos) <= 0.1f)
        {
            return true;
        }
        return false;
    }

    private void ChargeTowards(GameObject PM)
    {
        Vector2 targetPos = PM.transform.position;
		Vector2 difference = new Vector2(targetPos.x - m_Child.transform.position.x, targetPos.y - m_Child.transform.position.y);
        Vector2 direction = difference.normalized;

		m_Child.transform.Translate(direction * m_ecFSM.fSpeed);
    }
}

