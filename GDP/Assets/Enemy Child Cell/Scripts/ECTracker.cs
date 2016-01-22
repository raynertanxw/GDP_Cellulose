using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECTracker
{
	public static ECTracker s_Instance;

	public List<EnemyChildFSM> IdleCells;
	public List<EnemyChildFSM> ChargeChildCells;
	public List<EnemyChildFSM> ChargeMainCells;
	public List<EnemyChildFSM> LandmineCells;
	public List<EnemyChildFSM> TrickAttackCells;
	public List<EnemyChildFSM> AvoidCells;
	public List<EnemyChildFSM> DefendCells;
	
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
		IdleCells = new List<EnemyChildFSM>();
		ChargeChildCells = new List<EnemyChildFSM>();
		ChargeMainCells = new List<EnemyChildFSM>();
		LandmineCells = new List<EnemyChildFSM>();
		TrickAttackCells = new List<EnemyChildFSM>();
		AvoidCells = new List<EnemyChildFSM>();
		DefendCells = new List<EnemyChildFSM>();
	}
	
	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
