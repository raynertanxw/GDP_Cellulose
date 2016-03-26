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
				 playerNodeHoldText;
	
	void Start () 
	{
		if (instance == null)
			instance = this;

		tutorialPanelImage = transform.GetChild(0).GetComponent<Image>();
		playerNutrientText = transform.GetChild(1).GetComponent<Text>();
		playerNodeTapText = transform.GetChild(2).GetComponent<Text>();
		playerNodeHoldText = transform.GetChild(3).GetComponent<Text>();
	}

	// Update UI according to the current state
	void Update ()
	{
		// Tutorial Panel
		if (Tutorial.Instance ().tutorialState == TutorialState.PlayerNutrientWaiting ||
		    Tutorial.Instance ().tutorialState == TutorialState.PlayerNodeTapWaiting ||
		    Tutorial.Instance ().tutorialState == TutorialState.PlayerNodeHoldWaiting) {
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
	}
}