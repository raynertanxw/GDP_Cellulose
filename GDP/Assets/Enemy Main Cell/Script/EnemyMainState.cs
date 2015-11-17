using UnityEngine;
using System.Collections;

public abstract class EnemyMainState : MonoBehaviour 
{
	protected EnemyMainFSM m_EMFSM = null;

	public virtual void Enter () {}
	public virtual void Execute () {}
	public virtual void Exit () {}
}