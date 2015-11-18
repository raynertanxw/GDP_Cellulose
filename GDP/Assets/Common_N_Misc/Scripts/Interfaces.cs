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
    protected EnemyChildFSM m_ecFSM = null;
}