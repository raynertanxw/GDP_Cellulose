using UnityEngine;
using System.Collections;

public abstract class IState
{
	public virtual void Enter() {}
	public virtual void Execute() {}
	public virtual void Exit() {}
}

public abstract class IPCState : IState
{
	protected PlayerChildFSM m_pcFSM = null;
}

public abstract class IECState : IState
{
    protected GameObject child = null;
    protected EnemyChildFSM m_ecFSM = null;
}

public abstract class ISCState : IState
{
    protected PlayerSquadFSM m_scFSM = null;
}










public class IEMState : MonoBehaviour 
{
	protected EnemyMainFSM m_EMFSM = null;
	protected PlayerChildFSM m_PCFSM = null;
	
	public virtual void Enter () {}
	public virtual void Execute () {}
	public virtual void Exit () {}
}