using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialUI : MonoBehaviour 
{
	// Instance of the class
	private static TutorialUI instance;
	// Singleton
	public static TutorialUI Instance()
	{
		return instance;
	}

	private Image tutorialPanelImage;

	private Text playerNutrientText,
				 playerNodeTapText,
				 playerNodeHoldText,
				 playerNodeCommandText,
				 squadCaptainSpawnText;
	
	void Start () 
	{
		if (instance == null)
			instance = this;

		tutorialPanelImage = transform.GetChild(0).GetComponent<Image>();
		playerNutrientText = transform.GetChild(1).GetComponent<Text>();
		playerNodeTapText = transform.GetChild(2).GetComponent<Text>();
		playerNodeHoldText = transform.GetChild(3).GetComponent<Text>();
		playerNodeCommandText = transform.GetChild(4).GetComponent<Text>();
		squadCaptainSpawnText = transform.GetChild(5).GetComponent<Text>();
	}

	// Update UI according to the current state
	void Update ()
	{
		// Tutorial Panel
		if (Tutorial.Instance ().tutorialState == TutorialState.PlayerNutrientWaiting ||
		    Tutorial.Instance ().tutorialState == TutorialState.PlayerNodeTapWaiting ||
		    Tutorial.Instance ().tutorialState == TutorialState.PlayerNodeHoldWaiting ||
		    Tutorial.Instance ().tutorialState == TutorialState.PlayerNodeCommandWaiting ||
		    Tutorial.Instance ().tutorialState == TutorialState.SquadCaptainSpawnWaiting) {
			if (!tutorialPanelImage.enabled)
				tutorialPanelImage.enabled = true;
		} else {
			if (tutorialPanelImage.enabled)
				tutorialPanelImage.enabled = false;
		}
		// Player Nutrient Text
		if (Tutorial.Instance ().tutorialState == TutorialState.PlayerNutrientWaiting) {
			if (!playerNutrientText.enabled)
				playerNutrientText.enabled = true;
		} else {
			if (playerNutrientText.enabled)
				playerNutrientText.enabled = false;
		}
		// Player Node Tap Text
		if (Tutorial.Instance ().tutorialState == TutorialState.PlayerNodeTapWaiting) {
			if (!playerNodeTapText.enabled)
				playerNodeTapText.enabled = true;
		} else {
			if (playerNodeTapText.enabled)
				playerNodeTapText.enabled = false;
		}
		// Player Node Hold Text
		if (Tutorial.Instance ().tutorialState == TutorialState.PlayerNodeHoldWaiting) {
			if (!playerNodeHoldText.enabled)
				playerNodeHoldText.enabled = true;
		} else {
			if (playerNodeHoldText.enabled)
				playerNodeHoldText.enabled = false;
		}
		// Player Node Command Text
		if (Tutorial.Instance ().tutorialState == TutorialState.PlayerNodeCommandWaiting) {
			if (!playerNodeCommandText.enabled)
				playerNodeCommandText.enabled = true;
		} else {
			if (playerNodeCommandText.enabled)
				playerNodeCommandText.enabled = false;
		}
		// Squad Captain Spawn Text
		if (Tutorial.Instance ().tutorialState == TutorialState.SquadCaptainSpawnWaiting) {
			if (!squadCaptainSpawnText.enabled)
				squadCaptainSpawnText.enabled = true;
		} else {
			if (squadCaptainSpawnText.enabled)
				squadCaptainSpawnText.enabled = false;
		}
	}
}