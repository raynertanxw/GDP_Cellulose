using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECDefendState : IECState {

	//A boolean to track whether the position of the enemy child cell had reach the guiding position for the defence formation
	private bool m_bReachPos;
	private bool m_bKillClosestAttacker;

	private static float m_fDefendTime;
	private static float m_fMaxFormingAcceleration;
	private static float m_fMaxChaseAcceleration;
	private static float m_fMainScale;
	private static float m_fAutoDefendRange;
	
	private static bool m_bReturnToMain;
	private static bool m_bReachedMain;
	public static bool m_bThereIsDefenders;

	private Transform m_ECTransform;
	private Vector2 m_TargetPos;
	
	private static Transform m_EMTransform;
	private static Formation m_CurrentFormation;
	private static PlayerAttackMode m_AttackType;
	
	public static bool ReturningToMain
	{
		get { return m_bReturnToMain; }
	}

	//Constructor
	public ECDefendState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = _ecFSM.m_EMain;

		m_ECTransform = m_Child.transform;
		m_EMTransform = m_Main.transform;

		m_fMaxFormingAcceleration = 150f;
		m_fMaxChaseAcceleration = 500f;
		m_fAutoDefendRange = 7f;
		m_fDefendTime = 0f;
		m_bReturnToMain = false;
		m_bReachedMain = false;
		m_CurrentFormation = Formation.Empty;
	}

	public override void Enter()
	{
		m_bReachPos = false;
		m_bKillClosestAttacker = false;
		m_bThereIsDefenders = true;
		
		m_fMainScale = m_Main.transform.localScale.x;
		m_ecFSM.rigidbody2D.drag = 3f;
		
		ECTracker.s_Instance.DefendCells.Add(m_ecFSM);
		AudioManager.PlayECSoundEffect(EnemyChildSFX.Defend,m_ecFSM.Audio);
	}

	public override void Execute()
	{
		//If there is no formation for the enemy child cell to take, query the postionQuery to get which formation to get for the specific situation
		if(m_CurrentFormation == Formation.Empty)
		{
			m_AttackType = PositionQuery.Instance.GetMostSignificantAttack();
			m_CurrentFormation = PositionQuery.Instance.GetDefensiveFormation();
			
			FormationDatabase.Instance.RefreshDatabases(m_Main.GetComponent<EnemyMainFSM>().ECList);
			FormationDatabase.Instance.UpdateDatabaseFormation(m_CurrentFormation,m_fMainScale);
			if(m_CurrentFormation != Formation.QuickCircle){FormationDatabase.Instance.CheckOtherOverlapADRange();}
		}

		if(m_CurrentFormation == Formation.QuickCircle){m_TargetPos = FormationDatabase.Instance.CheckCircleOverlapADRange(FormationDatabase.Instance.GetTargetFormationPosition(m_CurrentFormation, m_Child));}
		if(m_CurrentFormation != Formation.QuickCircle){m_TargetPos = FormationDatabase.Instance.GetTargetFormationPosition(m_CurrentFormation, m_Child);}
	}

	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		if(!m_bReachPos && !m_bReturnToMain && !HasCellReachTargetPos(m_ECTransform.position,m_TargetPos) && !m_ecFSM.IsHittingSideWalls())
		{
			m_ecFSM.rigidbody2D.drag = 15f;//5f;
			Acceleration += SteeringBehavior.Seek(m_Child,m_TargetPos,300f);
		}
		else if(!m_bReachPos && !m_bReturnToMain && !HasCellReachTargetPos(m_ECTransform.position,m_TargetPos) && m_ecFSM.IsHittingSideWalls())
		{
			m_ecFSM.rigidbody2D.drag = 5f;
			Vector2 SeekVelo = SteeringBehavior.Seek(m_Child,m_TargetPos,300f);
		
			if((m_Child.transform.position.x < 0 && m_Main.GetComponent<Rigidbody2D>().velocity.x > 0) || (m_Child.transform.position.x >= 0 && m_Main.GetComponent<Rigidbody2D>().velocity.x <= 0))
			{
				Acceleration += SeekVelo;
			}
			else{Acceleration += new Vector2(0f, SeekVelo.y);}
			
			Acceleration += SteeringBehavior.ShakeOnSpot(m_Child,1f,8f);
		}
		else if(!m_bReachPos && !m_bReturnToMain && HasCellReachTargetPos(m_ECTransform.position,m_TargetPos))
		{
			m_bReachPos = true;
		}
		
		if(m_bReachPos && !m_bReturnToMain && !m_bKillClosestAttacker)
		{
			Acceleration += new Vector2(0f, m_Main.GetComponent<Rigidbody2D>().velocity.y) * 24f;
			Acceleration += SteeringBehavior.ShakeOnSpot(m_Child,1f,8f);
			
			//If at any point of time, the child cell got too far away from the given position, see back to the target position in the formation
			if(!HasCellReachTargetPos(m_Child.transform.position,m_TargetPos) && !m_bKillClosestAttacker)
			{
				m_ecFSM.rigidbody2D.drag = 5f;
				Acceleration += SteeringBehavior.Seek(m_Child,m_TargetPos,24f);
			}
		}

		//If there are attackers to the enemy main cell, seek to the closest attacking player child cells
		if(!m_bKillClosestAttacker && !IsThereNoAttackers() && IsPlayerChildPassingBy())
		{
			m_ecFSM.m_ChargeTarget = GetClosestAttacker();
		
			m_ecFSM.rigidbody2D.drag = 2.0f;
			m_ecFSM.rigidbody2D.velocity = Vector2.zero;
			
			m_bKillClosestAttacker = true;
			if(m_ecFSM.m_ChargeTarget == null)
			{
				m_bKillClosestAttacker = false;
				return;
			}
		}
		else if(m_bKillClosestAttacker)
		{
			if(m_ecFSM.m_ChargeTarget.GetComponent<PlayerChildFSM>().GetCurrentState() != PCState.ChargeMain && m_ecFSM.m_ChargeTarget.GetComponent<PlayerChildFSM>().GetCurrentState() != PCState.ChargeChild)
			{
				m_bKillClosestAttacker = false;
				m_ecFSM.m_ChargeTarget = null;
				return;
			}

			if(m_AttackType != PlayerAttackMode.BurstShot){ Acceleration += SteeringBehavior.Pursuit(m_Child,m_ecFSM.m_ChargeTarget,24f);}
			else if(m_AttackType == PlayerAttackMode.BurstShot){ Acceleration += SteeringBehavior.Pursuit(m_Child,m_ecFSM.m_ChargeTarget,70f);}

			m_ecFSM.RotateToHeading();
		}
		//If there is no attackers to the enemy main cell, increase the defend time. If that time reaches a limit, return the cells back to the main cell and transition back to idle state
		else if(!m_bKillClosestAttacker && m_bReachPos && !m_bReturnToMain && IsThereNoAttackers())
		{
			m_fDefendTime += Time.deltaTime;
			if(m_fDefendTime >= 20f)
			{
				m_bReturnToMain = true;
			}
		}

		//If the enemy child cells is return back to the enemy main cell but has not reach the position, continue seek back to the main cell
		if(!m_bReachedMain && m_bReturnToMain && !HasCellReachTargetPos(m_ECTransform.position,m_EMTransform.position))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_EMTransform.position,30f);
		}
		//if the enemy child cell returned back to the enemy main cell, transition it back to the idle state
		else if(!m_bReachedMain && m_bReturnToMain && HasCellReachTargetPos(m_ECTransform.position,m_EMTransform.position) && HasAllCellReachTargetPos(m_EMTransform.position))
		{
			m_bReachedMain = true;
		}
		else if(m_bReachedMain)
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0f);
		}

		//Clamp the acceleration velocity to a specific value and add that acceleration as a force to the enemy child cell
		if(!m_bReachPos){Acceleration = Vector2.ClampMagnitude(Acceleration,m_fMaxFormingAcceleration);}
		else if(m_bReachPos && m_AttackType != PlayerAttackMode.BurstShot){ Acceleration = Vector2.ClampMagnitude(Acceleration,m_fMaxFormingAcceleration);}
		else if(m_bReachPos && m_AttackType == PlayerAttackMode.BurstShot){ Acceleration = Vector2.ClampMagnitude(Acceleration, m_fMaxChaseAcceleration);}

		m_ecFSM.rigidbody2D.AddForce(Acceleration,ForceMode2D.Force);

		//if the enemy child cell had reached the targeted position in the formation, rotate the enemy child cell randomly. Else, rotate it based on the direction of force applied
		if(m_bReachPos)
		{
			m_ecFSM.RandomRotation();
		}
		else if(!m_ecFSM.IsHittingSideWalls())
		{
			m_ecFSM.RotateToHeading();
		}
	}

	public override void Exit()
	{
		m_fDefendTime = 0f;

		//When exiting, if there is no more defending child cells, empty out the CurrentFormation variable
		if(ECTracker.s_Instance == null || ECTracker.s_Instance.DefendCells.Count <= 1)
		{
			m_CurrentFormation = Formation.Empty;
			m_fDefendTime = 0f;
			m_bReturnToMain = false;
			m_bThereIsDefenders = false;
			m_bReachedMain = false;
		}

		//Reset the velocity and force applied to the enemy child cell
		m_ecFSM.rigidbody2D.velocity = Vector2.zero;
		m_ecFSM.rigidbody2D.drag = 0.0f;

		m_ecFSM.m_ChargeTarget = null;
		FormationDatabase.Instance.ReturnFormationPos(m_Child);
		ECTracker.s_Instance.DefendCells.Remove(m_ecFSM);
	}

	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTargetPos(Vector2 _CellPos,Vector2 _TargetPos)
	{
		return (Utility.Distance(_CellPos, _TargetPos) <= 0.1f) ? true : false;
	}

	//A function that return a boolean on whether all the cells had reached the given position in the perimeter
	private bool HasAllCellReachTargetPos(Vector2 _Pos)
	{
		List<EnemyChildFSM> ECList = ECTracker.s_Instance.DefendCells;
		for(int i = 0; i < ECList.Count; i++)
		{
			if(!HasCellReachTargetPos(ECList[i].transform.position,_Pos))
			{
				return false;
			}
		}
		return true;
	}

	//A function that return a boolean on whether there is any player child cell that passed by this enemy child cell
	private bool IsPlayerChildPassingBy()
	{
		Collider2D[] PasserBy = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x * m_fAutoDefendRange,Constants.s_onlyPlayerChildLayer);
		return (PasserBy.Length > 0) ? true : false;
	}

	//A function that return if there is no attacking player child cells
	private bool IsThereNoAttackers()
	{
		for(int i = 0; i < PlayerChildFSM.childrenInAttack.Length - 1; i++)
		{
			if(PlayerChildFSM.childrenInAttack[i] == -1){break;}
			
			if(PlayerChildFSM.childrenInAttack[i] != null){return false;}
		}

		return true;
	}

	//A function that return a GameObject variable on the closest attacking player child cell
	private GameObject GetClosestAttacker()
	{
		GameObject ClosestAttacker = null;
		float Distance = Mathf.Infinity;
		
		if(IsthereBurstShot())
		{
			for(int i = 0; i < PlayerChildFSM.playerChildPool.Length; i++)
			{
				if(PlayerChildFSM.playerChildPool[i].attackMode == PlayerAttackMode.BurstShot && Utility.Distance(PlayerChildFSM.playerChildPool[i].transform.position,m_ecFSM.m_EMain.transform.position) < Distance)
				{
					ClosestAttacker = PlayerChildFSM.playerChildPool[i].gameObject;
					Distance = Utility.Distance(PlayerChildFSM.playerChildPool[i].transform.position,m_ecFSM.m_EMain.transform.position);
				}
			}
		}
		else
		{
			GameObject[] PlayerChilds = GameObject.FindGameObjectsWithTag(Constants.s_strPlayerChildTag);
			float ECtoPCDistance = 0f;
			PCState PCCurrentState = PCState.Idle;
			
			for(int i = 0; i < PlayerChilds.Length; i++)
			{
				ECtoPCDistance = Utility.Distance(PlayerChilds[i].transform.position,m_Child.transform.position);
				PCCurrentState = PlayerChilds[i].GetComponent<PlayerChildFSM>().GetCurrentState();
				
				if((PCCurrentState == PCState.ChargeChild || PCCurrentState == PCState.ChargeMain) && ECtoPCDistance < Distance)
				{
					ClosestAttacker = PlayerChilds[i];
					Distance = ECtoPCDistance;
				}
			}
		}
	
		return ClosestAttacker;
	}

	private bool IsthereBurstShot()
	{
		for(int i = 0; i < PlayerChildFSM.playerChildPool.Length; i++)
		{
			if(PlayerChildFSM.playerChildPool[i].GetCurrentState() == PCState.ChargeMain && PlayerChildFSM.playerChildPool[i].attackMode == PlayerAttackMode.BurstShot)
			{
				return true;
			}
		}
		return false;
	}

	//A function that return a boolean on whether if all cells had gathered together in the enemy main cell
	private bool HasAllCellsGathered()
	{
		List<EnemyChildFSM> DefendingCells = ECTracker.s_Instance.DefendCells;
		for(int i = 0; i < DefendingCells.Count; i++)
		{
			if(!HasCellReachTargetPos(DefendingCells[i].transform.position,m_Main.transform.position))
			{
				return false;
			}
		}
		return true;
	}
	
	private bool HasTargetGoOffRange(GameObject _Target)
	{
		if(_Target.transform.position.y - m_Main.transform.position.y > 3f)
		{
			return true;
		}
		return false;
	}
}
