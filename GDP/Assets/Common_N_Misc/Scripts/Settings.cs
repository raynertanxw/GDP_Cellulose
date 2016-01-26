using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour
{
	#region Player Level Variables
	public const int s_nPlayerMaxChildCount = 100;
	public const int s_nPlayerMaxNutrientCount = 10;

	public const int s_nPlayerInitialResourceCount = 1000;
	public const int s_nPlayerInitialHealth = 50;

	public const int s_nPlayerNutrientPerBlock = 10;
	public const int s_nPlayerChildSpawnCost = 5;

	public const int s_nPlayerSqaudCaptainChildCost = 10;
	public const int s_nPlayerActionBurstShotChildCost = 5;
	public const int s_nPlayerActionSwarmTargetChildCost = 10;
	public const int s_nPlayerActionScatterShotChildCost = 20;
	#endregion

	#region Enemy Main Level Variables
	public const float fEnemyMainMaxY = 9.6f;
	public const float fEnemyMainMinY = -1.5f;

	public const int nEnemyMainInitialHealthLV1 = 50;
	#endregion
}
