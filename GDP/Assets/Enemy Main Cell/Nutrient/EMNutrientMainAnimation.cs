﻿using UnityEngine;
using System.Collections;

public class EMNutrientMainAnimation : MonoBehaviour
{
	public static bool bCanHaveFootstep = true;
	public static GameObject previousParent;
	[SerializeField]
	private bool bHaveFootstep;

	public GameObject footstepPool;

	private Transform footstep0;
	private Transform footstep1;
	private Transform footstep2;
	private Transform footstep3;
	private Transform footstep4;
	private Transform footstep5;
	private Transform footstep6;
	private Transform footstep7;
	private Transform footstep8;
	private Transform footstep9;

	private Transform[] footstepArr;
	private int nNewFootstep;

	float fElapsedTime;
	float fFootstepRate;

	void Start ()
	{
		bHaveFootstep = false;

		if ((EMNutrientMainAnimation.bCanHaveFootstep == true || EMNutrientMainAnimation.previousParent == null) && gameObject.activeSelf) 
		{
			EMNutrientMainAnimation.bCanHaveFootstep = false;
			bHaveFootstep = true;
			EMNutrientMainAnimation.previousParent = this.gameObject;
		}

		if (bHaveFootstep) 
		{
			footstepArr = new Transform[10];
			footstep0 = footstepPool.transform.GetChild (0);
			footstep1 = footstepPool.transform.GetChild (1);
			footstep2 = footstepPool.transform.GetChild (2);
			footstep3 = footstepPool.transform.GetChild (3);
			footstep4 = footstepPool.transform.GetChild (4);
			footstep5 = footstepPool.transform.GetChild (5);
			footstep6 = footstepPool.transform.GetChild (6);
			footstep7 = footstepPool.transform.GetChild (7);
			footstep8 = footstepPool.transform.GetChild (8);
			footstep9 = footstepPool.transform.GetChild (9);

			footstepArr [0] = footstep0;
			footstepArr [1] = footstep1;
			footstepArr [2] = footstep2;
			footstepArr [3] = footstep3;
			footstepArr [4] = footstep4;
			footstepArr [5] = footstep5;
			footstepArr [6] = footstep6;
			footstepArr [7] = footstep7;
			footstepArr [8] = footstep8;
			footstepArr [9] = footstep9;

			fElapsedTime = 0f;
			fFootstepRate = 1f;

			nNewFootstep = 0;
		}
	}

	void Update () 
	{
		if (bHaveFootstep)
			UpdateFootstep ();
	}

	void UpdateFootstep ()
	{
		if (gameObject.activeSelf) 
		{
			fElapsedTime += Time.deltaTime;

			if (fElapsedTime >= fFootstepRate) 
			{
				fElapsedTime = 0f;

				if (footstepArr [nNewFootstep] != null)
					footstepArr [nNewFootstep].position = transform.position;

				if (nNewFootstep == 9)
					nNewFootstep = 0;
				else
					nNewFootstep++;
			}
		} 
		else
			fElapsedTime = 0f;
	}
}