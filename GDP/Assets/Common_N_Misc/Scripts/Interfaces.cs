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
	private PlayerChildFSM m_pcFSM = null;
}