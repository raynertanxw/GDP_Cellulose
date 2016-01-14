using UnityEngine;
using System.Collections;

public class EMNutrientGeneration : MonoBehaviour 
{
	int nLevelNo;
	int nNumOfNutrient;
	
	void Start () 
	{
		nLevelNo = 1;
		nNumOfNutrient = (int)Random.Range (4f * Mathf.Sqrt ((float)nLevelNo), 8f * Mathf.Sqrt ((float)nLevelNo));
		if (nNumOfNutrient > EMNutrientMainAgent.AgentList.Count)
			nNumOfNutrient = EMNutrientMainAgent.AgentList.Count;
		if (EMNutrientMainAgent.AgentList.Count - nNumOfNutrient > 0) 
		{
			for (int i = 0; i < EMNutrientMainAgent.AgentList.Count - nNumOfNutrient; i++) 
			{
				EMNutrientMainAgent.AgentList [i].ActivateOrDeactivate (false);
			}
		}
	}

	void Update () 
	{
	
	}
}