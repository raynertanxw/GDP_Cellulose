using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECDefendState : IECState {

	//A boolean to track whether the position of the enemy child cell had reach the guiding position for the defence formation
	private bool bReachPos;

	//A static boolean that tracked whether the enemy child cells had gathered together
	private static bool bGathered;

	//A boolean that tracked whether any adjustment is needed to the enemy child cell while moving
	private bool bAdjustNeeded;

	//A boolean that state whether the enemy child cell is retreating back to the enemy main cell after the defend state is completed and transitioning to idle state
	private static bool bReturnToMain;
	
	public static bool bThereIsDefenders;

	//A vector2 to store the defending position that the enemy child cell need to move to
	private Vector2 m_TargetPos;

	//A float to store how the enemy child cell had been in the defensive formation when there is no attacking player cells nearby
	private static float fDefendTime;

	//A float to store the maximum amount of acceleration that can be enforced on the enemy child cell
	private static float fMaxFormingAcceleration;
	private static float fMaxChaseAcceleration;

	//A float that scale the values used in the defend state by the local state of the enemy main cell
	private static float fMainScale;

	//A formation variable to store whether what current formation that the defending enemy child cell will take
	private static Formation CurrentFormation;

	private bool bKillClosestAttacker;

	private static float fAutoDefendRange;
	
	private static bool bReachedMain;
	
	private Transform ECTransform;
	private static Transform EMTransform;

	private static PlayerAttackMode AttackType;
	
	public static bool ReturningToMain
	{
		get { return bReturnToMain; }
	}

	//Constructor
	public ECDefendState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = _ecFSM.m_EMain;

		ECTransform = m_Child.transform;
		EMTransform = m_Main.transform;

		fMaxFormingAcceleration = 150f;
		fMaxChaseAcceleration = 500f;
		fAutoDefendRange = 7f;//6f;//25f;//20f;//12f//9f;//4f;//5.5f;
		fDefendTime = 0f;
		bReturnToMain = false;
		bReachedMain = false;
		bGathered = false;
		CurrentFormation = Formation.Empty;
	}

	public override void Enter()
	{
		bReachPos = false;
		bAdjustNeeded = false;
		
		bKillClosestAttacker = false;
		bThereIsDefenders = true;

		//fDefendTime = 0f;
		fMainScale = m_Main.transform.localScale.x * 0.75f;

		m_ecFSM.rigidbody2D.drag = 3f;
		ECTracker.s_Instance.DefendCells.Add(m_ecFSM);
	}

	public override void Execute()
	{
		//If there is no formation for the enemy child cell to take, query the postionQuery to get which formation to get for the specific situation
		if(CurrentFormation == Formation.Empty)
		{
			AttackType = PositionQuery.Instance.GetMostSignificantAttack();
			CurrentFormation = PositionQuery.Instance.GetDefensiveFormation();
			FormationDatabase.Instance.RefreshDatabases(m_Main.GetComponent<EnemyMainFSM>().ECList);
			FormationDatabase.Instance.UpdateDatabaseFormation(CurrentFormation,fMainScale);
			FormationDatabase.Instance.CheckOverlapPlayerADRange();
		}

		//Based on the current formation, get a specific target position within that formation
		m_TargetPos = FormationDatabase.Instance.GetTargetFormationPosition(CurrentFormation, m_Child);
	}

	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		if(!bReachPos && !bAdjustNeeded && !bReturnToMain && !HasCellReachTargetPos(ECTransform.position,m_TargetPos) && !m_ecFSM.IsHittingSideWalls())
		{
			m_ecFSM.rigidbody2D.drag = 15f;//5f;
			Acceleration += SteeringBehavior.Seek(m_Child,m_TargetPos,300f);//27
		}
		else if(!bReachPos && !bAdjustNeeded && !bReturnToMain && !HasCellReachTargetPos(ECTransform.position,m_TargetPos) && m_ecFSM.IsHittingSideWalls())
		{
			m_ecFSM.rigidbody2D.velocity = m_Main.GetComponent<Rigidbody2D>().velocity;
		}
		else if(!bReachPos && !bReturnToMain && HasCellReachTargetPos(ECTransform.position,m_TargetPos))
		{
			bReachPos = true;
		}
		
		if(bReachPos && !bReturnToMain && !bKillClosestAttacker)
		{
			Acceleration += new Vector2(0f, m_Main.GetComponent<Rigidbody2D>().velocity.y) * 24f;
			Acceleration += SteeringBehavior.ShakeOnSpot(m_Child,1f,8f);
			
			//If at any point of time, the child cell got too far away from the given position, see back to the target position in the formation
			if(!HasCellReachTargetPos(m_Child.transform.position,m_TargetPos) && !bKillClosestAttacker && !m_ecFSM.IsHittingSideWalls())
			{
				m_ecFSM.rigidbody2D.drag = 5f;
				Acceleration += SteeringBehavior.Seek(m_Child,m_TargetPos,24f);
			}
		}

		//If there are attackers to the enemy main cell, seek to the closest attacking player child cells
		if(!bKillClosestAttacker && !IsThereNoAttackers() && IsPlayerChildPassingBy())
		{
			m_ecFSM.m_ChargeTarget = GetClosestAttacker();
		
			m_ecFSM.rigidbody2D.drag = 2.0f;
			m_ecFSM.rigidbody2D.velocity = Vector2.zero;
			
			bKillClosestAttacker = true;
			//Utility.CheckEmpty<GameObject>(m_ecFSM.m_ChargeTarget);
			if(m_ecFSM.m_ChargeTarget == null)
			{
				bKillClosestAttacker = false;
				return;
			}
		}
		else if(bKillClosestAttacker)
		{
			if(m_ecFSM.m_ChargeTarget.GetComponent<PlayerChildFSM>().GetCurrentState() != PCState.ChargeMain && m_ecFSM.m_ChargeTarget.GetComponent<PlayerChildFSM>().GetCurrentState() != PCState.ChargeChild)
			{
				bKillClosestAttacker = false;
				m_ecFSM.m_ChargeTarget = null;
				return;
			}
			//Debug.Log("pursuit");
			if(AttackType != PlayerAttackMode.BurstShot){ Acceleration += SteeringBehavior.Pursuit(m_Child,m_ecFSM.m_ChargeTarget,24f);}
			else if(AttackType == PlayerAttackMode.BurstShot){ Acceleration += SteeringBehavior.Pursuit(m_Child,m_ecFSM.m_ChargeTarget,70f);}
			//Debug.Log("magnitude: " + Acceleration.magnitude);

			m_ecFSM.RotateToHeading();
		}
		//If there is no attackers to the enemy main cell, increase the defend time. If that time reaches a limit, return the cells back to the main cell and transition back to idle state
		else if(!bKillClosestAttacker && bReachPos && !bReturnToMain && IsThereNoAttackers())
		{
			fDefendTime += Time.deltaTime;
			if(fDefendTime >= 20f)
			{
				bReturnToMain = true;
			}
		}

		//If the enemy child cells is return back to the enemy main cell but has not reach the position, continue seek back to the main cell
		if(!bReachedMain && bReturnToMain && !HasCellReachTargetPos(ECTransform.position,EMTransform.position))
		{
			//Debug.Log("return to main");
			Acceleration += SteeringBehavior.Seek(m_Child,EMTransform.position,30f);
		}
		//if the enemy child cell returned back to the enemy main cell, transition it back to the idle state
		else if(!bReachedMain && bReturnToMain && HasCellReachTargetPos(ECTransform.position,EMTransform.position) && HasAllCellReachTargetPos(EMTransform.position))
		{
			bReachedMain = true;
		}
		else if(bReachedMain)
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0f);
			//ECIdleState.ImmediateCohesion();
		}

		//Clamp the acceleration velocity to a specific value and add that acceleration as a force to the enemy child cell
		if(!bReachPos){Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxFormingAcceleration);}
		else if(bReachPos && AttackType != PlayerAttackMode.BurstShot){ Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxFormingAcceleration);}
		else if(bReachPos && AttackType == PlayerAttackMode.BurstShot){ Acceleration = Vector2.ClampMagnitude(Acceleration, fMaxChaseAcceleration);}

		m_ecFSM.rigidbody2D.AddForce(Acceleration,ForceMode2D.Force);

		//if the enemy child cell had reached the targeted position in the formation, rotate the enemy child cell randomly. Else, rotate it based on the direction of force applied
		if(bReachPos)
		{
			m_ecFSM.RandomRotation(0.85f);
		}
		else
		{
			m_ecFSM.RotateToHeading();
		}
	}

	public override void Exit()
	{
		fDefendTime = 0f;

		//When exiting, if there is no more defending child cells, empty out the CurrentFormation variable
		if(ECTracker.s_Instance.DefendCells.Count <= 1)
		{
			CurrentFormation = Formation.Empty;
			fDefendTime = 0f;
			bReturnToMain = false;
			bGathered = false;
			bThereIsDefenders = false;
			bReachedMain = false;
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
		Collider2D[] PasserBy = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x * fAutoDefendRange,Constants.s_onlyPlayerChildLayer);
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
			//Debug.Log("check for burst target");
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
			//Debug.Log("no burst target");
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
