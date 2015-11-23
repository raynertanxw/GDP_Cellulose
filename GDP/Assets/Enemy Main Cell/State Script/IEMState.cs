using UnityEngine;
using System.Collections;

public class IEMState : MonoBehaviour 
{
	protected EnemyMainFSM m_EMFSM = null;
	protected PlayerChildFSM m_PCFSM = null;

	public virtual void Enter () {}
	public virtual void Execute () {}
	public virtual void Exit () {}
}