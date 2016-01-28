using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndGamePanel : MonoBehaviour
{
	private static EndGamePanel s_Instance = null;
	public static EndGamePanel Instance { get { return s_Instance; } }

	private CanvasGroup winCanvasGrp, loseCanvasGrp;
	private Button winNextLevelBtn, winMenuBtn, loseRetryBtn, loseMenuBtn;
	private float fButtonDisableTiming = 2.0f;

	void Awake()
	{
		if (s_Instance == null)
			s_Instance = this;
		else
			Destroy(this.gameObject);

		winCanvasGrp = transform.GetChild(0).GetComponent<CanvasGroup>();
		winNextLevelBtn = transform.GetChild(0).GetChild(2).GetComponent<Button>();
		winMenuBtn = transform.GetChild(0).GetChild(3).GetComponent<Button>();
		loseCanvasGrp = transform.GetChild(1).GetComponent<CanvasGroup>();
		loseRetryBtn = transform.GetChild(1).GetChild(3).GetComponent<Button>();
		loseMenuBtn = transform.GetChild(1).GetChild(4).GetComponent<Button>();

		SetEndGamePanelVisibility(false);
		SetEndGameButtonsInteractable(false);
	}

	public void SetEndGamePanelVisibility(bool _visible)
	{
		if (_visible)
		{
			if (GameManager.Instance.bPlayerWon == true)
			{
				SetWinGameVisibility(true);
				SetLoseGameVisibility(false);
			}
			else
			{
				SetWinGameVisibility(false);
				SetLoseGameVisibility(true);
			}

			player_control.Instance.DeselectAllCtrls();
			Invoke("EnableEndGameButtons", fButtonDisableTiming);
		}
		else
		{
			SetWinGameVisibility(false);
			SetLoseGameVisibility(false);
		}
	}

	private void SetWinGameVisibility(bool _visible)
	{
		if (_visible)
		{
			winCanvasGrp.interactable = true;
			winCanvasGrp.blocksRaycasts = true;
			winCanvasGrp.alpha = 1f;
		}
		else
		{
			winCanvasGrp.interactable = false;
			winCanvasGrp.blocksRaycasts = false;
			winCanvasGrp.alpha = 0f;
		}
	}

	private void SetLoseGameVisibility(bool _visible)
	{
		if (_visible)
		{
			loseCanvasGrp.interactable = true;
			loseCanvasGrp.blocksRaycasts = true;
			loseCanvasGrp.alpha = 1f;
		}
		else
		{
			loseCanvasGrp.interactable = false;
			loseCanvasGrp.blocksRaycasts = false;
			loseCanvasGrp.alpha = 0f;
		}
	}

	private void SetEndGameButtonsInteractable(bool _interactable)
	{
		winNextLevelBtn.interactable = _interactable;
		winMenuBtn.interactable = _interactable;
		loseRetryBtn.interactable = _interactable;
		loseMenuBtn.interactable = _interactable;
	}

	private void EnableEndGameButtons()
	{
		SetEndGameButtonsInteractable(true);
	}



	#region OnClick functions
	public void ButtonBackToMenu()
	{
		SceneManager.LoadScene(0);
	}

	public void ButtonNextLevel()
	{
		SceneManager.LoadScene(Application.loadedLevel);
	}

	public void ButtonRetry()
	{
		SceneManager.LoadScene(Application.loadedLevel);
	}
	#endregion










	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
