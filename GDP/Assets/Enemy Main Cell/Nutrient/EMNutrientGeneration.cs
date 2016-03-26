using UnityEngine;
using System.Collections;

public class EMNutrientGeneration : MonoBehaviour 
{
	int nLevelNo;
	int nNumOfNutrient;
	
	void Start () 
	{
		// Get the number of level
		nLevelNo = Level_Manager.LevelID;
		// Randomize the number of main nutrient
		nNumOfNutrient = (int)Random.Range (2f * Mathf.Sqrt ((float)nLevelNo), 3f * Mathf.Sqrt ((float)nLevelNo));
		// Max number of nutrient
		if (nNumOfNutrient > 10)
			nNumOfNutrient = 10;

		if (EMNutrientMainAgent.AgentList != null)
		{
			// deactive nutrient objects
			if (nNumOfNutrient > 0) {
				for (int i = 0; i < 10 - nNumOfNutrient; i++) {
					EMNutrientMainAgent.AgentList [i].ActivateOrDeactivate (false);
				}
			}
		}
	}

	void Update () 
	{

	}
}