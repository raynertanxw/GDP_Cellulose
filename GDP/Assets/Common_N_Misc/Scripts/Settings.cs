using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour
{
    #region Player Level Variables
    public static int s_nPlayerInitialHealth = 50;
    public static int s_nPlayerInitialResourceCount = 200;

	public static int s_nPlayerNutrientPerBlock = 10;
	public static float s_fPlayerNutrientChance = 0.25f;
	public static int s_nPlayerChildSpawnCost = 5;

	public static int s_nPlayerSqaudCaptainChildCost = 15;
	public static int s_nPlayerActionBurstShotChildCost = 5;
	public static int s_nPlayerActionSwarmTargetChildCost = 10;
	public static int s_nPlayerActionScatterShotChildCost = 20;
    #endregion

    #region Squad Captain Level Variables
    public static float s_fAggressiveToDefensive = 0.5f;
    public static float s_fMinimumCooldown = 1f;
    public static float s_fMaximumCooldown = 2f;
    public static float s_fThinkCooldown = 0.5f;
    #endregion

    #region Enemy Main Level Variables
	public static int s_nEnemyMainInitialHealth = 50;
	public static int s_nEnemyMainInitialNutrientNum = 50;
	public static int s_nEnemyMainInitialChildCellNum = 5;
	public static int s_nEnemyMainInitialAggressiveness = 3;

	public static float s_fEnemyMainInitialVertSpeed = 0.05f;
	public static float s_fEnemyMainMinHiriSpeed = 0.05f;

	public static float s_fDefaultStunTime = 5f;
	public static float s_fDefaultStunTolerance = 5f;
    #endregion

    #region Enemy Child Level Variables
    public static int s_nEnemyChildCountCap = 50;

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

    #region Aesthetics Level Variables
    public static Color s_EnvironmentColor = Color.white;
    public static float s_fSideWallSpeed = 15f;
    public static float s_BackgroundSpeed = 12.5f;
    #endregion
}
