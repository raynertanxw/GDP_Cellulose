using UnityEngine;
using System.Collections;

public class EMDifficulty
{
	// Instance of the class
	private static EMDifficulty instance;
	// Singleton
	public static EMDifficulty Instance()
	{
		return instance;
	}

	[SerializeField]
	private float fHealthDiff;					// Difficulty factor affected by the current health of the enemy main cell
	private float fMaxHealthInfluence;
	private int nPrecedingHealth;
	[SerializeField]
	private float fNutrientDiff;				// Difficulty factor affected by the current num of nutrient of the enemy main cell
	private float fMaxNutrientInfluence;
	private int nPrecedingNutrient;
	[SerializeField]
	private float fLevelDiff;					// Difficulty factor affected by the current level
	[SerializeField]
	private float fCurrentDiff;
	public float CurrentDiff { get { return fCurrentDiff; } }

	void Start () 
	{
		if (instance == null)
			instance = this;
		// Initialization of difficulty and factors
		fHealthDiff = 1f;
		fNutrientDiff = 1f;
		fLevelDiff = 1f;
		fCurrentDiff = 1f;
		fMaxHealthInfluence = .5f;
		fMaxNutrientInfluence = .5f;
		// Initialization of current health and num of nutrient
		if (EnemyMainFSM.Instance ().Health != 0)
			nPrecedingHealth = EnemyMainFSM.Instance ().Health;
		if (EMController.Instance ().NutrientNum != 0)
			nPrecedingNutrient = EMController.Instance ().NutrientNum;
	}

	void Update () 
	{
		#region Update of health and num of nutrient, and difficulty factors
		// Health Update
		if (nPrecedingHealth != EnemyMainFSM.Instance ().Health)
		{
			nPrecedingHealth = EnemyMainFSM.Instance ().Health;
			HealthDiffUpdate ();
		}
		// Nutrient Update
		if (nPrecedingNutrient != EMController.Instance ().NutrientNum)
		{
			nPrecedingNutrient = EMController.Instance ().NutrientNum;
			NutrientDiffUpdate ();
		}
		#endregion
	}

	void HealthDiffUpdate ()
	{
		// Max fHealthDiff = 1f + fMaxHealthInfluence
		fHealthDiff = 1f + fMaxHealthInfluence / Mathf.Sqrt (Mathf.Sqrt ((float)EnemyMainFSM.Instance ().Health));
	}

	void NutrientDiffUpdate ()
	{
		// Max fNutrientDiff = 1f + fMaxNutrientInfluence
		fNutrientDiff = 1f + fMaxNutrientInfluence / Mathf.Sqrt (Mathf.Sqrt ((float)EnemyMainFSM.Instance ().Health) * 2f) / Mathf.Sqrt (Mathf.Sqrt ((2f)));
	}

	void LevelDiffUpdate ()
	{

	}

	void CurrentDiffUpdate ()
	{

	}
}