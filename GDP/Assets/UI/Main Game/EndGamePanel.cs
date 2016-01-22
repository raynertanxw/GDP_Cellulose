using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndGamePanel : MonoBehaviour
{
	private static EndGamePanel s_Instance = null;
	public static EndGamePanel Instance { get { return s_Instance; } }

	private CanvasGroup canvasGrp;
	private Text endGameText, nextRetryText;

	void Awake()
	{
		if (s_Instance == null)
			s_Instance = this;
		else
			Destroy(this.gameObject);

		canvasGrp = GetComponent<CanvasGroup>();
		endGameText = transform.GetChild(0).GetComponent<Text>();
		nextRetryText = transform.GetChild(1).GetChild(0).GetComponent<Text>();

		SetEndGamePanelVisibility(false);
	}

	public void SetEndGamePanelVisibility(bool _visible)
	{
		if (_visible)
		{
			canvasGrp.interactable = true;
			canvasGrp.blocksRaycasts = true;
			canvasGrp.alpha = 1f;
		}
		else
		{
			canvasGrp.interactable = false;
			canvasGrp.blocksRaycasts = false;
			canvasGrp.alpha = 0f;
		}
	}




	#region OnClick functions
	public void ButtonBackToMenu()
	{

	}

	public void ButtonNextOrRetryLevel()
	{

	}
	#endregion
}
