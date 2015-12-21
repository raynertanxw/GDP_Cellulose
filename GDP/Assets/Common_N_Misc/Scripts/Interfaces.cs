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
    protected GameObject m_Child = null;
    protected GameObject m_Main = null;
    protected EnemyChildFSM m_ecFSM = null;
}

public abstract class ISCState : IState
{
    protected SquadChildFSM m_scFSM = null;
}

public abstract class IPSState : IState 
{
    protected PlayerSquadFSM m_psFSM = null;
}

public abstract class IEMState : IState
{
	protected EnemyMainFSM m_EMFSM = null;
	
	public virtual void Enter () {}
	public virtual void Execute () {}
	public virtual void Exit () {}
}