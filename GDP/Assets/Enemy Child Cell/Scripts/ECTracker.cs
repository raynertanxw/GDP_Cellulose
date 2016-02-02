using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECTracker
{
	public static ECTracker s_Instance;

	private List<EnemyChildFSM> m_IdleCells;
	private List<EnemyChildFSM> m_AttackingCells;
	private List<EnemyChildFSM> m_ChargeChildCells;
	private List<EnemyChildFSM> m_ChargeMainCells;
	private List<EnemyChildFSM> m_LandmineCells;
	private List<EnemyChildFSM> m_TrickAttackCells;
	private List<EnemyChildFSM> m_AvoidCells;
	private List<EnemyChildFSM> m_DefendCells;
	
	public List<EnemyChildFSM> IdleCells{get{return m_IdleCells;}}
	public List<EnemyChildFSM> AttackingCells{get{return m_AttackingCells;}}
	public List<EnemyChildFSM> ChargeChildCells{get{return m_ChargeChildCells;}}
	public List<EnemyChildFSM> ChargeMainCells{get{return m_ChargeMainCells;}}
	public List<EnemyChildFSM> LandmineCells{get{return m_LandmineCells;}}
	public List<EnemyChildFSM> TrickAttackCells{get{return m_TrickAttackCells;}}
	public List<EnemyChildFSM> AvoidCells{get{return m_AvoidCells;}}
	public List<EnemyChildFSM> DefendCells{get{return m_DefendCells;}}
	
	public static ECTracker Instance
	{
		get
		{
			if(s_Instance == null)
			{
				s_Instance = new ECTracker();
			}
			return s_Instance;
		}
	}
	
	public ECTracker()
	{
		m_IdleCells = new List<EnemyChildFSM>();
		m_AttackingCells = new List<EnemyChildFSM>();
		m_ChargeChildCells = new List<EnemyChildFSM>();
		m_ChargeMainCells = new List<EnemyChildFSM>();
		m_LandmineCells = new List<EnemyChildFSM>();
		m_TrickAttackCells = new List<EnemyChildFSM>();
		m_AvoidCells = new List<EnemyChildFSM>();
		m_DefendCells = new List<EnemyChildFSM>();
	}
	
	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
