﻿using UnityEngine;
using System.Collections;

public enum TutorialState {
	Start, 
	PlayerNutrientSpawned, PlayerNutrientWaiting, PlayerNutrientCollected, 
	PlayerNodeTapWaiting, PlayerNodeTapCompleted,
	PlayerNodeHoldWaiting, PlayerNodeHoldCompleted,
	PlayerNodeCommandWaiting, PlayerNodeCommandCompleted,
	SquadCaptainSpawnWaiting, SquadCaptainSpawnCompleted,
	EnemyMainProductionWaiting, EnemyMainProductionCompleted,
	EnemyMainDefendWaiting, EnemyMainDefendCompleted,
	EnemyMainCautiousAttackWaiting, EnemyMainCautiousAttackCompleted,
	EnemyMainAggressiveAttackWaiting, EnemyMainAggressiveAttackCompleted,
	EnemyMainLandmineWaiting, EnemyMainLandmineCompleted,
	Ending, End};

public class Tutorial : MonoBehaviour 
{
	// Instance of the class
	private static Tutorial instance;
	// Singleton
	public static Tutorial Instance()
	{
		return instance;
	}

	public static bool bIsTutorial = false;

	public GameObject playerNutrient;

	public GameObject[] EnemyNutrients = new GameObject[10];

	public TutorialState tutorialState;
	
	void Start () 
	{
		if (instance == null)
			instance = this;

		Init ();
	}

	public void Init ()
	{
		bIsTutorial = true;
		
		tutorialState = (int)TutorialState.Start;
		
		// Deactive all enemy nutrients by default
		EnemyNutrients = GameObject.FindGameObjectsWithTag ("EnemyMainNutrient");
		for (int i = 0; i < 10; i++)
			EnemyNutrients [i].SetActive (false);
	}

	void Update () 
	{
		// Update the tutorial state
		if (tutorialState == TutorialState.Start && playerNutrient != null) {
			StartCoroutine (PlayerNutrientSpawned ());
			tutorialState = TutorialState.PlayerNutrientSpawned;
		} 
		else if (tutorialState == TutorialState.PlayerNutrientCollected && Time.timeScale != 1f) {
			Time.timeScale = 1f;
			StartCoroutine (PlayerNodeTapWaiting ());
		}
		else if (tutorialState == TutorialState.PlayerNodeTapCompleted && Time.timeScale != 1f) {
			Time.timeScale = 1f;
			StartCoroutine (PlayerNodeHoldWaiting ());
		}
		else if (tutorialState == TutorialState.PlayerNodeHoldCompleted && Time.timeScale != 1f) {
			Time.timeScale = 1f;
			StartCoroutine (PlayerNodeCommandWaiting ());
		}
		else if (tutorialState == TutorialState.PlayerNodeCommandCompleted && Time.timeScale != 1f) {
			Time.timeScale = 1f;
			StartCoroutine (SquadCaptainSpawnWaiting ());
		}
		else if (tutorialState == TutorialState.SquadCaptainSpawnCompleted && Time.timeScale != 1f) {
			Time.timeScale = 1f;
			StartCoroutine (EnemyMainProductionWaiting ());
		}
		else if (tutorialState == TutorialState.EnemyMainProductionCompleted && Time.timeScale != 1f) {
			Time.timeScale = 1f;
			StartCoroutine (EnemyMainDefendWaiting ());
		}
		else if (tutorialState == TutorialState.EnemyMainDefendCompleted && Time.timeScale != 1f) {
			Time.timeScale = 1f;
			StartCoroutine (EnemyMainCautiousAttackWaiting ());
		}
		else if (tutorialState == TutorialState.EnemyMainCautiousAttackCompleted && Time.timeScale != 1f) {
			Time.timeScale = 1f;
			StartCoroutine (EnemyMainAggressiveAttackWaiting ());
		}
		else if (tutorialState == TutorialState.EnemyMainAggressiveAttackCompleted && Time.timeScale != 1f) {
			Time.timeScale = 1f;
			StartCoroutine (EnemyMainLandmineWaiting ());
		}
	}

	IEnumerator PlayerNutrientSpawned ()
	{
		yield return new WaitForSeconds (2.25f);
		Tutorial.Instance().tutorialState = TutorialState.PlayerNutrientWaiting;
		Time.timeScale = 0.001f;
	}

	IEnumerator PlayerNodeTapWaiting ()
	{
		yield return new WaitForSeconds (3f);
		Tutorial.Instance().tutorialState = TutorialState.PlayerNodeTapWaiting;
		Time.timeScale = 0.001f;
	}

	IEnumerator PlayerNodeHoldWaiting ()
	{
		yield return new WaitForSeconds (3f);
		Tutorial.Instance().tutorialState = TutorialState.PlayerNodeHoldWaiting;
		Time.timeScale = 0.5f;
	}

	IEnumerator PlayerNodeCommandWaiting ()
	{
		yield return new WaitForSeconds (3f);
		Tutorial.Instance().tutorialState = TutorialState.PlayerNodeCommandWaiting;
		Time.timeScale = 0.001f;
	}

	IEnumerator SquadCaptainSpawnWaiting ()
	{
		yield return new WaitForSeconds (3f);
		Tutorial.Instance().tutorialState = TutorialState.SquadCaptainSpawnWaiting;
		Time.timeScale = 0.5f;
	}

	IEnumerator EnemyMainProductionWaiting ()
	{
		yield return new WaitForSeconds (3f);
		Tutorial.Instance().tutorialState = TutorialState.EnemyMainProductionWaiting;
		// Prohibit other transitions in the next 6 seconds
		StartCoroutine(EMTransition.Instance().TransitionAvailability(6f));
		// Transition to Production state for demonstration
		EnemyMainFSM.Instance ().ChangeState (EMState.Production);
		Time.timeScale = 0.75f;
		yield return new WaitForSeconds (4f);
		Tutorial.Instance().tutorialState = TutorialState.EnemyMainProductionCompleted;
	}

	IEnumerator EnemyMainDefendWaiting ()
	{
		yield return new WaitForSeconds (2f);
		Tutorial.Instance().tutorialState = TutorialState.EnemyMainDefendWaiting;
		StartCoroutine(EMTransition.Instance().TransitionAvailability(6f));
		EnemyMainFSM.Instance ().ChangeState (EMState.Defend);
		Time.timeScale = 0.75f;
		yield return new WaitForSeconds (4f);
		Tutorial.Instance().tutorialState = TutorialState.EnemyMainDefendCompleted;
	}

	IEnumerator EnemyMainCautiousAttackWaiting ()
	{
		yield return new WaitForSeconds (2f);
		Tutorial.Instance().tutorialState = TutorialState.EnemyMainCautiousAttackWaiting;
		StartCoroutine(EMTransition.Instance().TransitionAvailability(6f));
		EnemyMainFSM.Instance ().ChangeState (EMState.CautiousAttack);
		Time.timeScale = 0.75f;
		yield return new WaitForSeconds (4f);
		Tutorial.Instance().tutorialState = TutorialState.EnemyMainCautiousAttackCompleted;
	}

	IEnumerator EnemyMainAggressiveAttackWaiting ()
	{
		yield return new WaitForSeconds (2f);
		Tutorial.Instance().tutorialState = TutorialState.EnemyMainAggressiveAttackWaiting;
		StartCoroutine(EMTransition.Instance().TransitionAvailability(6f));
		EnemyMainFSM.Instance ().ChangeState (EMState.AggressiveAttack);
		Time.timeScale = 0.75f;
		yield return new WaitForSeconds (4f);
		Tutorial.Instance().tutorialState = TutorialState.EnemyMainAggressiveAttackCompleted;
	}

	IEnumerator EnemyMainLandmineWaiting ()
	{
		yield return new WaitForSeconds (2f);
		Tutorial.Instance().tutorialState = TutorialState.EnemyMainLandmineWaiting;
		StartCoroutine(EMTransition.Instance().TransitionAvailability(6f));
		EnemyMainFSM.Instance ().ChangeState (EMState.Landmine);
		Time.timeScale = 0.75f;
		yield return new WaitForSeconds (4f);
		Tutorial.Instance().tutorialState = TutorialState.EnemyMainLandmineCompleted;
		StartCoroutine (Ending ());
	}

	IEnumerator Ending ()
	{
		yield return new WaitForSeconds (2f);
		Tutorial.Instance ().tutorialState = TutorialState.Ending;
		yield return new WaitForSeconds (4f);
		Tutorial.Instance ().tutorialState = TutorialState.End;
		Application.LoadLevel ("Main_Menu");
	}
}