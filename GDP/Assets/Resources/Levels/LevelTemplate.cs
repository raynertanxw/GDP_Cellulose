using UnityEngine;
using System.Collections;

public class LevelTemplate : ScriptableObject
{
	[Header("Player Data")]
	public int PlayerInitialResourceCount;
	public int PlayerInitialHealth;

	[Header("Squad Captain Data")]
	public float SquadCaptainAggressiveNDefensiveSlider;
	public float SquadCaptainProductionSpeed;

	[Header("Enemy Data")]
	public int EnemyInitialResourceCount;
	public int EnemyInitialHealth;
	public int EnemyInitialChildCount;
	public float EnemyProductionSpeed;
	public float EnemyMovementSpeed;
	public float EnemyKnockbackSpeed;
	public float EnemyMainAggression;
	[Range(1.0f, 2.0f)]
	public float EnemyDifficulty;

	[Header("Aesthetics Data")]
	public Color EnemyColor;
	public Color EnvironmentColor;
	public float SideWallSpeed;
	public float BackgroundSpeed;
}
