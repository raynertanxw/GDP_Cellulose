using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour
{
	public static Vector3 s_farfarAwayVector = new Vector3(0f, 1000f, 0f);

	#region Layer Masks
	public static LayerMask s_onlyEnemeyChildLayer = 1 << LayerMask.NameToLayer("EnemyChild");
	public static LayerMask s_onlyPlayerChildLayer = 1 << LayerMask.NameToLayer("PlayerChild");
	#endregion

	#region Tags
	public const string s_strPlayerTag = "Player";
	public const string s_strPlayerChildTag = "PlayerChild";
	public const string s_strEnemyTag = "Enemy";
	public const string s_strEnemyChildTag = "EnemyChild";
	public const string s_strEnemyMainNutrient = "EnemyMainNutrient";
	public const string s_strEnemyMiniNutrient = "EnemyMiniNutrient";
	#endregion
}
