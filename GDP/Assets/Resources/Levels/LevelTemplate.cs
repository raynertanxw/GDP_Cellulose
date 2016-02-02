using UnityEngine;
using System.Collections;

public class LevelTemplate : ScriptableObject
{
    #region Player Level Variables
    [Header("Player Data")]
    [Range(50, 200)] [Tooltip("Player’s starting health.")]
    public int nPlayerInitialHealth = 50;
    [Range(0, 1000)] [Tooltip("Player’s starting nutrient(resources) amount.")]
    public int nPlayerInitialResourceCount = 200;

    [Range(10, 25)] [Tooltip("The number of nutrient units given per nutrient block.")]
    public int nPlayerNutrientPerBlock = 10;
    [Range(5, 10)] [Tooltip("The number of nutrient units needed to spawn 1 player child cell.")]
    public int nPlayerChildSpawnCost = 5;

    [Range(10, 25)] [Tooltip("The number of player child cells needed to spawn in the player squad captain unit.")]
    public int nPlayerSqaudCaptainChildCost = 10;
    [Range(3, 5)] [Tooltip("The number of player child cells needed for BurstShot command.")]
    public int nPlayerActionBurstShotChildCost = 5;
    [Range(5, 15)] [Tooltip("The number of player child cells needed for SwarmTarget command.")]
    public int nPlayerActionSwarmTargetChildCost = 10;
    [Range(10, 30)] [Tooltip("The number of player child cells needed for ScatterShot command.")]
    public int nPlayerActionScatterShotChildCost = 20;
    #endregion

    #region Squad Captain Level Variables
    [Header("Squad Captain Data")]
    [Range(0.0f, 1.0f)] [Tooltip("The slider to depict the aggressiveness and the defensiveness of the squad captain.")]
    public float fAggressiveToDefensive = 0.5f;
    [Range(1.0f, 3.0f)] [Tooltip("The fastest that the squad captain can spawn children, this is the speed of production when all squad children is in produce state.")]
    public float fMinimumCooldown = 1f;
    [Range(5.0f, 7.0f)] [Tooltip("The slowest that the squad captain can spawn children, this is the speed of production when 1 squad children is in produce state.")]
    public float fMaximumCooldown = 5f;
    [Range(0.5f, 2.0f)] [Tooltip("The speed at which the squad captain thinks, this will not affect quick reactions.")]
    public float fThinkCooldown = 0.5f;
    #endregion

    #region Enemy Main Level Variables
    [Header("Enemy Main Data")]
    [Range(50, 200)] [Tooltip("Enemy Main initial health.")]
    public int nEnemyMainInitialHealth = 50;
    [Range(50, 100)] [Tooltip("Enemy Main initial nutrient number.")]
    public int nEnemyMainInitialNutrientNum = 50;
    [Range(5, 20)] [Tooltip("Enemy Main initial child cell number.")]
    public int nEnemyMainInitialChildCellNum = 5;
    [Range(3, 8)] [Tooltip("Enemy Main initial aggressiveness.")]
    public int nEnemyMainInitialAggressiveness = 3;

    [Range(0.05f, 0.1f)] [Tooltip("Enemy Main initial vertical speed.")]
    public float fEnemyMainInitialVertSpeed = 0.05f;
    [Range(0.05f, 0.1f)] [Tooltip("Enemy Main initial horizontal speed.")]
    public float fEnemyMainMinHiriSpeed = 0.05f;

    [Range(3.0f, 5.0f)] [Tooltip("Enemy Main initial stun duration.")]
    public float fDefaultStunTime = 5f;
    [Range(4.0f, 10.0f)] [Tooltip("Enemy Main initial stun tolerance (max number of hits can receive before getting stunned).")]
    public float fDefaultStunTolerance = 5f;
    #endregion

    #region Enemy Child Level Variables
    [Header("Enemy Child Data")]
    [Range(0, 100)] [Tooltip("Maximum amount of enemy child cells the enemy main can produce and maintained at the same time.")]
    public int nEnemyChildCountCap = 50;

    [Range(0.0f, 1.0f)] [Tooltip("Amount of priority that Player Left Node will have in target selection process.")]
    public float fEnemyTargetLeftNodeRequirement = 0.1f;
    [Range(0.0f, 1.0f)] [Tooltip("Amount of priority that Player Right Node will have in target selection process.")]
    public float fEnemyTargetRightNodeRequirement = 0.1f;
    [Range(0.0f, 1.0f)] [Tooltip("Amount of priority that Squad Captain Cell will have in target selection process.")]
    public float fEnemyTargetSquadCaptRequirement = 0.1f;

    [Range(0.0f, 1.0f)] [Tooltip("Amount of priority that Enemy Charge Attack will have in Attack selection process.")]
    public float fEnemyAttackChargeRequirement = 0.1f;
    [Range(0.0f, 1.0f)] [Tooltip("Amount of priority that Enemy Landmine will have in Attack selection process.")]
    public float fEnemyAttackLandmineRequirement = 0.1f;
    [Range(0.0f, 1.0f)] [Tooltip("Amount of priority that Enemy Trick Attack will have in Attack Selection process.")]
    public float fEnemyAttackTrickAttackRequirement = 0.1f;

    [Range(1.0f, 3.0f)] [Tooltip("Amount of emphasis that the Enemy Main Cell will place against defending Burst Shot (Eg. If there is are attacking cells that have varying attack patterns, the enemy main cell will focus on defend against any BurstShot Attacks).")]
    public float fEnemyDefendBurstSignificancy = 1.0f;
    [Range(1.0f, 3.0f)] [Tooltip("Amount of emphasis that the Enemy Main Cell will place against defending Swarm attack (Eg. If there is are attacking cells that have varying attack patterns, the enemy main cell will focus on defend against any Swarm Attacks).")]
    public float fEnemyDefendSwarmSignificancy = 1.0f;
    [Range(1.0f, 3.0f)] [Tooltip("Amount of emphasis that the Enemy Main Cell will place against defending Scatter shot (Eg. If there is are attacking cells that have varying attack patterns, the enemy main cell will focus on defend against any Scatter Attacks).")]
    public float fEnemyDefendScatterSignificancy = 1.0f;

    [Range(1.0f, 4.0f)] [Tooltip("Amount of weighting placed for the Quick Circle formation to choosen for defending (Quick-Circle formation will have child cells form circle around the enemy main cell).")]
    public float fEnemyDefendQCWeight = 1.0f;
    [Range(1.0f, 4.0f)] [Tooltip("Amount of weighting placed for the Reverse Cover formation to choosen for defending (Reverse Cover formation will have child cells form reverse V in front of the enemy main cell).")]
    public float fEnemyDefendRCWeight = 1.0f;
    [Range(1.0f, 4.0f)] [Tooltip("Amount of weighting placed for the Turtle formation to choosen for defending (Turtle formation will have child cells form a thick layer of child cells at the front of enemy main cells).")]
    public float fEnemyDefendTurtleWeight = 1.0f;
    [Range(1.0f, 4.0f)] [Tooltip("Amount of weighting placed for the Ladder formation to choosen for defending (Ladder formation will have child cells form a thick layer of child cells at the left and right of enemy main cells).")]
    public float fEnemyDefendLadderWeight = 1.0f;
    #endregion

    #region Aesthetics Level Variables
    [Header("Aesthetics Data")]
    [Tooltip("Colour of the enemy for the level.")]
    public Color EnemyColor = Color.white;
    [Tooltip("Colour of the background and walls for the level.")]
    public Color EnvrionmentColor = Color.white;
    [Range(5.0f, 20.0f)] [Tooltip("Scrolling speed of the walls.")]
    public float fSideWallSpeed = 15f;
    [Range(2.5f, 15.0f)] [Tooltip("Scrolling speed of the background.")]
    public float BackgroundSpeed = 12.5f;
    #endregion
}
