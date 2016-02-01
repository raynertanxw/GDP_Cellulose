using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour
{
	public static Vector3 s_farfarAwayVector = new Vector3(0f, 1000f, 0f);

    #region Game Related
    public const int s_nPlayerMaxChildCount = 100;
    public const int s_nPlayerMaxNutrientCount = 10;
    public const float s_fEnemyMainMaxY = 9.6f;
    public const float s_fEnemyMainMinY = -1.5f;
    public const int s_nEnemyMaxChildCount = 100;
    #endregion

    #region Layer Masks
    public static LayerMask s_onlyEnemeyChildLayer = 1 << LayerMask.NameToLayer("EnemyChild");
    public static LayerMask s_onlyEnemyMainLayer = 1 << LayerMask.NameToLayer("EnemyMain");
	public static LayerMask s_onlyPlayerChildLayer = 1 << LayerMask.NameToLayer("PlayerChild");
    public static LayerMask s_onlyPlayerMainLayer = 1 << LayerMask.NameToLayer("PlayerMain");

    public static LayerMask s_onlyWallLayer = 1 << LayerMask.NameToLayer("Wall");
	#endregion

	#region Tags
	public const string s_strPlayerTag = "Player";
	public const string s_strPlayerChildTag = "PlayerChild";
	public const string s_strEnemyTag = "Enemy";
	public const string s_strEnemyChildTag = "EnemyChild";
	public const string s_strEnemyMainNutrient = "EnemyMainNutrient";
	public const string s_strEnemyMiniNutrient = "EnemyMiniNutrient";
	#endregion

	#region Coroutines
	public const string s_strAnimateInSpawnCtrl = "AnimateInSpawnCtrl";
	public const string s_strFadeOutInfoPanel = "FadeOutInfoPanel";
	public const string s_FadeOutCanvasGroup = "FadeOutCanvasGroup";
	#endregion
}
