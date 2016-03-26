using UnityEngine;
using System.Collections;

public enum TutorialState {
	Start, 
	PlayerNutrientSpawned, PlayerNutrientWaiting, PlayerNutrientCollected, 
	PlayerNodeTapWaiting, PlayerNodeTapCompleted,
	PlayerNodeHoldWaiting, PlayerNodeHoldCompleted};

public class Tutorial : MonoBehaviour 
{
	// Instance of the class
	private static Tutorial instance;
	// Singleton
	public static Tutorial Instance()
	{
		return instance;
	}

	public GameObject playerNutrient;

	public GameObject[] EnemyNutrients = new GameObject[10];

	public TutorialState tutorialState;
	
	void Start () 
	{
		if (instance == null)
			instance = this;

		tutorialState = (int)TutorialState.Start;

		// Deactive all enemy nutrients by default
		EnemyNutrients = GameObject.FindGameObjectsWithTag ("EnemyMainNutrient");
		for (int i = 0; i < 10; i++)
			EnemyNutrients [i].SetActive (false);
	}

	void Update () 
	{
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
}