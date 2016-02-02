﻿using UnityEngine;
using System.Collections;

public class EMNutrientGeneration : MonoBehaviour 
{
	int nLevelNo;
	int nNumOfNutrient;
	
	void Start () 
	{
		nLevelNo = Level_Manager.LevelID;
		// Randomize the number of main nutrient
		nNumOfNutrient = (int)Random.Range (2f * Mathf.Sqrt ((float)nLevelNo), 3f * Mathf.Sqrt ((float)nLevelNo));
		if (nNumOfNutrient > 10)
			nNumOfNutrient = 10;

		if (EMNutrientMainAgent.AgentList != null)
		{
			if (nNumOfNutrient > 0) {
				for (int i = 0; i < nNumOfNutrient; i++) {
					EMNutrientMainAgent.AgentList [i].ActivateOrDeactivate (false);
				}
			}
		}
	}

	void Update () 
	{
	
	}
}