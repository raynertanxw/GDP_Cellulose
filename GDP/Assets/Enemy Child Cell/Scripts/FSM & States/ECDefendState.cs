﻿using UnityEngine;
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
	private float fMaxAcceleration;

	//A float that scale the values used in the defend state by the local state of the enemy main cell
	private float fMainScale;

	//A formation variable to store whether what current formation that the defending enemy child cell will take
	private static Formation CurrentFormation;

	private bool bKillClosestAttacker;

	private float fAutoDefendRange;

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

		fMaxAcceleration = 40f;
		fAutoDefendRange = 12f;//9f;//4f;//5.5f;
		fDefendTime = 0f;
		bReturnToMain = false;
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

		m_Child.GetComponent<Rigidbody2D>().drag = 3f;
	}

	public override void Execute()
	{
		//If there is no formation for the enemy child cell to take, query the postionQuery to get which formation to get for the specific situation
		if(CurrentFormation == Formation.Empty)
		{
			CurrentFormation = PositionQuery.Instance.GetDefensiveFormation();
			FormationDatabase.Instance.RefreshDatabases(m_Main.GetComponent<EnemyMainFSM>().ECList);
			FormationDatabase.Instance.UpdateDatabaseFormation(CurrentFormation,fMainScale);
		}

		//Based on the current formation, get a specific target position within that formation
		m_TargetPos = FormationDatabase.Instance.GetTargetFormationPosition(CurrentFormation, m_Child);

		//If all enemy child cells have gathered, start transition them to the idle state
		if(HasAllCellsGathered())
		{
			bGathered = true;
		}
	}

	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		//If the enemy child cell had not reach the enemy main cell when all the enemy child cells had not gathered at the enemy main cell, seek them to the enemy main cell while rotating them to face the direction of travel
		if(!HasCellReachTargetPos(m_Main.transform.position) && !HasAllCellsGathered() && !bGathered && !bReturnToMain)
		{
			m_ecFSM.RotateToHeading();
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,35f);
		}

		//If the enemy child cell had not reach the targeted position in the formation, continue seek them to that position
		if(!HasCellReachTargetPos(m_TargetPos) && bGathered && !bReachPos && !bAdjustNeeded && !bReturnToMain)
		{
			m_Child.GetComponent<Rigidbody2D>().drag = 5f;
			Acceleration += SteeringBehavior.Seek(m_Child,m_TargetPos,40f);//27
		}
		else if(!HasCellReachTargetPos(m_TargetPos) && bGathered && !bReachPos && bAdjustNeeded && !bReturnToMain)
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_TargetPos,24f);
		}
		else if(HasCellReachTargetPos(m_TargetPos) && bGathered && !bReachPos && !bReturnToMain)
		{
			bReachPos = true;
		}

		//If the enemy child cell had reach the targeted position in the formation, continue follow the enemy main cell and move around the given targeted positio n
		if(bReachPos && !bReturnToMain && !bKillClosestAttacker)
		{
			Acceleration += new Vector2(0f, m_Main.GetComponent<Rigidbody2D>().velocity.y) * 24f;
			Acceleration += SteeringBehavior.ShakeOnSpot(m_Child,1f,8f);

			//If at any point of time, the child cell got too far away from the given position, see back to the target position in the formation
			if(!HasCellReachTargetPos(m_TargetPos) && !bKillClosestAttacker)
			{
				m_Child.GetComponent<Rigidbody2D>().drag = 5f;
				Acceleration += SteeringBehavior.Seek(m_Child,m_TargetPos,24f);
			}
		}

		//If there are attackers to the enemy main cell, seek to the closest attacking player child cells
		if(!IsThereNoAttackers() && IsPlayerChildPassingBy() && !bKillClosestAttacker)
		{
			bKillClosestAttacker = true;
			m_Child.GetComponent<Rigidbody2D>().drag = 2.3f;
			m_Child.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			m_ecFSM.m_ChargeTarget = GetClosestAttacker();
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
			Acceleration += SteeringBehavior.Seek(m_Child,m_ecFSM.m_ChargeTarget.transform.position,26f);
		}
		//If there is no attackers to the enemy main cell, increase the defend time. If that time reaches a limit, return the cells back to the main cell and transition back to idle state
		else if(IsThereNoAttackers() && bGathered && bReachPos && !bReturnToMain && !bKillClosestAttacker)
		{
			//Debug.Log(fDefendTime);
			fDefendTime += Time.deltaTime;
			if(fDefendTime >= 10f)
			{
				bReturnToMain = true;
			}
		}

		//If the enemy child cells is return back to the enemy main cell but has not reach the position, continue seek back to the main cell
		if(bReturnToMain && !HasCellReachTargetPos(m_Main.transform.position))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,15f);
		}
		//if the enemy child cell returned back to the enemy main cell, transition it back to the idle state
		else if(bReturnToMain && HasCellReachTargetPos(m_Main.transform.position) && HasAllCellReachTargetPos(m_Main.transform.position))
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0.0f);
		}

		//Clamp the acceleration velocity to a specific value and add that acceleration as a force to the enemy child cell
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration,ForceMode2D.Force);

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
		if(GetDefendingCells().Count <= 1)
		{
			CurrentFormation = Formation.Empty;
			fDefendTime = 0f;
			bReturnToMain = false;
			bGathered = false;
			bThereIsDefenders = false;
		}

		//Reset the velocity and force applied to the enemy child cell
		m_Child.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		m_Child.GetComponent<Rigidbody2D>().drag = 0.0f;

		m_ecFSM.m_ChargeTarget = null;
		FormationDatabase.Instance.ReturnFormationPos(m_Child);
	}

	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		return (Utility.Distance(m_Child.transform.position, _Pos) <= 0.05f) ? true : false;
	}

	//A function that return a boolean on whether all the cells had reached the given position in the perimeter
	private bool HasAllCellReachTargetPos(Vector2 _Pos)
	{
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		for(int i = 0; i < ECList.Count; i++)
		{
			if(ECList[i].CurrentStateEnum == ECState.Idle && !HasCellReachTargetPos(_Pos))
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
		GameObject[] PlayerChilds = GameObject.FindGameObjectsWithTag(Constants.s_strPlayerChildTag);
		for(int i = 0; i < PlayerChilds.Length; i++)
		{
			PCState CurrentState = PlayerChilds[i].GetComponent<PlayerChildFSM>().GetCurrentState();
			if(CurrentState == PCState.ChargeChild || CurrentState == PCState.ChargeMain)
			{
				return false;
			}
		}
		return true;
	}

	//A function that return a GameObject variable on the closest attacking player child cell
	private GameObject GetClosestAttacker()
	{
		GameObject[] PlayerChilds = GameObject.FindGameObjectsWithTag(Constants.s_strPlayerChildTag);
		GameObject ClosestAttacker = null;
		float Distance = Mathf.Infinity;
		
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

		return ClosestAttacker;
	}

	//A function that return all the enemy child cells that are in defend state
	private List<GameObject> GetDefendingCells()
	{
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		List<GameObject> Defenders = new List<GameObject>();
		
		for(int i = 0; i < ECList.Count; i++)
		{
			if(ECList[i].CurrentStateEnum == ECState.Defend)
			{
				Defenders.Add(ECList[i].gameObject);
			}
		}
		return Defenders;
	}

	//A function that return a boolean on whether if all cells had gathered together in the enemy main cell
	private bool HasAllCellsGathered()
	{
		List<GameObject> DefendingCells = GetDefendingCells();
		for(int i = 0; i < DefendingCells.Count; i++)
		{
			if(!HasCellReachTargetPos(m_Main.transform.position))
			{
				return false;
			}
		}
		return true;
	}
}
