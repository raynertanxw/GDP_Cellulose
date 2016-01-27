﻿using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour
{
	#region Player Level Variables
	public static int s_nPlayerMaxChildCount = 100;
	public static int s_nPlayerMaxNutrientCount = 10;

	public static int s_nPlayerInitialResourceCount = 200;
	public static int s_nPlayerInitialHealth = 50;

	public static int s_nPlayerNutrientPerBlock = 10;
	public static int s_nPlayerChildSpawnCost = 5;

	public static int s_nPlayerSqaudCaptainChildCost = 10;
	public static int s_nPlayerActionBurstShotChildCost = 5;
	public static int s_nPlayerActionSwarmTargetChildCost = 10;
	public static int s_nPlayerActionScatterShotChildCost = 20;
	#endregion

	#region Enemy Main Level Variables
	public static float fEnemyMainMaxY = 9.6f;
	public static float fEnemyMainMinY = -1.5f;

	public static int nEnemyMainInitialHealthLV1 = 50;
	#endregion
	
	#region Enemy Child Level Variables
	public static int s_nEnemyMaxChildCount = 100;
	
	public static float s_fEnemyTargetLeftNodeRequirement = 0.1f;
	public static float s_fEnemyTargetRightNodeRequirement = 0.1f;
	public static float s_fEnemyTargetSquadCaptRequirement = 0.1f;
	
	public static float s_fEnemyAttackChargeRequirement = 0.1f;
	public static float s_fEnemyAttackLandmineRequirement = 0.1f;
	public static float s_fEnemyAttackTrickAttackRequirement = 0.1f;
	
	public static float s_fEnemyDefendBurstSignificancy = 1.0f;
	public static float s_fEnemyDefendSwarmSignificancy = 1.0f;
	public static float s_fEnemyDefendScatterSignificancy = 1.0f;
	
	public static float s_fEnemyDefendQCWeight = 1.0f;
	public static float s_fEnemyDefendRCWeight = 1.0f;
	public static float s_fEnemyDefendTurtleWeight = 1.0f;
	public static float s_fEnemyDefendLadderWeight = 1.0f;
	#endregion
}
