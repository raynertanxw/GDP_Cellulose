using UnityEngine;
using System.Collections;

public class Level_Manager
{
	public static void LoadLevel(int _level)
	{
		string path = "Levels/LEVEL_" + _level.ToString();
		LevelTemplate levelData = Resources.Load(path) as LevelTemplate;
		SetSettings(levelData);
	}

	public static void LoadTutorial()
	{
		string path = "Levels/LEVEL_TUTORIAL";
		LevelTemplate tutorialData = Resources.Load(path) as LevelTemplate;
		SetSettings(tutorialData);
	}

	private static void SetSettings(LevelTemplate _data)
	{
        // Player Level Variables
        Settings.s_nPlayerInitialHealth = _data.nPlayerInitialHealth;
        Settings.s_nPlayerInitialResourceCount = _data.nPlayerInitialResourceCount;

        Settings.s_nPlayerNutrientPerBlock = _data.nPlayerNutrientPerBlock;
        Settings.s_nPlayerChildSpawnCost = _data.nPlayerChildSpawnCost;

        Settings.s_nPlayerSqaudCaptainChildCost = _data.nPlayerSqaudCaptainChildCost;
        Settings.s_nPlayerActionBurstShotChildCost = _data.nPlayerActionBurstShotChildCost;
        Settings.s_nPlayerActionSwarmTargetChildCost = _data.nPlayerActionSwarmTargetChildCost;
        Settings.s_nPlayerActionScatterShotChildCost = _data.nPlayerActionScatterShotChildCost;

        // Squad Captain Level Variables
        Settings.s_fAggressiveToDefensive = _data.fAggressiveToDefensive;
        Settings.s_fMinimumCooldown = _data.fMinimumCooldown;
        Settings.s_fMaximumCooldown = _data.fMaximumCooldown;
        Settings.s_fThinkCooldown = _data.fThinkCooldown;

        // Enemy Main Level Variables
        Settings.s_nEnemyMainInitialHealth = _data.nEnemyMainInitialHealth;
        Settings.s_nEnemyMainInitialNutrientNum = _data.nEnemyMainInitialNutrientNum;
        Settings.s_nEnemyMainInitialChildCellNum = _data.nEnemyMainInitialChildCellNum;
        Settings.s_nEnemyMainInitialAggressiveness = _data.nEnemyMainInitialAggressiveness;

        Settings.s_fEnemyMainInitialVertSpeed = _data.fEnemyMainInitialVertSpeed;
        Settings.s_fEnemyMainMinHiriSpeed = _data.fEnemyMainMinHiriSpeed;

        Settings.s_fDefaultStunTime = _data.fDefaultStunTime;
        Settings.s_fDefaultStunTolerance = _data.fDefaultStunTolerance;

        // Enemy Child Level Variables
        Settings.s_nEnemyChildCountCap = _data.nEnemyChildCountCap;

        Settings.s_fEnemyTargetLeftNodeRequirement = _data.fEnemyTargetLeftNodeRequirement;
        Settings.s_fEnemyTargetRightNodeRequirement = _data.fEnemyTargetRightNodeRequirement;
        Settings.s_fEnemyTargetSquadCaptRequirement = _data.fEnemyTargetSquadCaptRequirement;

        Settings.s_fEnemyAttackChargeRequirement = _data.fEnemyAttackChargeRequirement;
        Settings.s_fEnemyAttackLandmineRequirement = _data.fEnemyAttackLandmineRequirement;
        Settings.s_fEnemyAttackTrickAttackRequirement = _data.fEnemyAttackTrickAttackRequirement;

        Settings.s_fEnemyDefendBurstSignificancy = _data.fEnemyDefendBurstSignificancy;
        Settings.s_fEnemyDefendSwarmSignificancy = _data.fEnemyDefendSwarmSignificancy;
        Settings.s_fEnemyDefendScatterSignificancy = _data.fEnemyDefendScatterSignificancy;

        Settings.s_fEnemyDefendQCWeight = _data.fEnemyDefendQCWeight;
        Settings.s_fEnemyDefendRCWeight = _data.fEnemyDefendRCWeight;
        Settings.s_fEnemyDefendTurtleWeight = _data.fEnemyDefendTurtleWeight;
        Settings.s_fEnemyDefendLadderWeight = _data.fEnemyDefendLadderWeight;

        // Aesthetics Level Variables
        Settings.s_EnemyColor = _data.EnemyColor;
        Settings.s_EnvrionmentColor = _data.EnvrionmentColor;
        Settings.s_fSideWallSpeed = _data.fSideWallSpeed;
        Settings.s_BackgroundSpeed = _data.BackgroundSpeed;
    }
}
