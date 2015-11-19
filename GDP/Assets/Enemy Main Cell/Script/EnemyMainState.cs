using UnityEngine;
using System.Collections;

public class EnemyMainState : MonoBehaviour 
{
	protected EnemyMainFSM m_EMFSM = null;
	protected PlayerChildFSM m_PCFSM = null;

	public virtual void Enter () {}
	public virtual void Execute () {}
	public virtual void Exit () {}
}