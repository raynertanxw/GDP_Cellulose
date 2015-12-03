﻿using UnityEngine;
using System.Collections;

public class ECChargeCState : IECState {

    private float fChargeSpeed;
    public GameObject m_Target;

    public ECChargeCState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_ecFSM = _ecFSM;
		m_Child = _childCell;
        fChargeSpeed = 0.3f;
    }

    public override void Enter()
    {
		m_Target = FindTargetChild();
    }

    public override void Execute()
    {
		if (HasCellReachTargetPos(m_Target.transform.position) == false)
        {
			ChargeTowards(m_Target);
        }
        else
        {
			MessageDispatcher.Instance.DispatchMessage(m_Child, m_ecFSM.gameObject, MessageType.Dead, 0.0);
        }
    }

    public override void Exit()
    {
    }

    private bool CheckIfTargetIsAvailable(GameObject _GO)
    {
        GameObject[] childCells = GameObject.FindGameObjectsWithTag("EnemyChild");
        for (int i = 0; i < childCells.Length; i++)
        {
			if (childCells[i].GetComponent<ECChargeCState>().m_Target == _GO)
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
			if (CheckIfTargetIsAvailable(childCells[i]) && (Vector2.Distance(m_Child.transform.position, childCells[i].transform.position)) < distanceBetween)
            {
				distanceBetween = Vector2.Distance(m_Child.transform.position, childCells[i].transform.position);
                closestChild = childCells[i];
            }
        }

        return closestChild;
    }


    private bool HasCellReachTargetPos(Vector2 _pos)
    {
		if (Vector2.Distance(m_Child.transform.position, _pos) <= 0.1f)
        {
            return true;
        }
        return false;
    }

    private void ChargeTowards(GameObject _PC)
    {
		Vector2 targetPos = _PC.transform.position;
		Vector2 difference = new Vector2(targetPos.x - m_Child.transform.position.x, targetPos.y - m_Child.transform.position.y);
        Vector2 direction = difference.normalized;

		m_Child.transform.Translate(direction * m_ecFSM.fSpeed);
    }
}
