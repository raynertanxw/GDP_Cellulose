using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndGamePanel : MonoBehaviour
{
	private static EndGamePanel s_Instance = null;
	public static EndGamePanel Instance { get { return s_Instance; } }

	private CanvasGroup winCanvasGrp, loseCanvasGrp;

	void Awake()
	{
		if (s_Instance == null)
			s_Instance = this;
		else
			Destroy(this.gameObject);

		winCanvasGrp = transform.GetChild(0).GetComponent<CanvasGroup>();
		loseCanvasGrp = transform.GetChild(1).GetComponent<CanvasGroup>();

		SetEndGamePanelVisibility(false);
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



	#region OnClick functions
	public void ButtonBackToMenu()
	{
		Application.LoadLevel(0);
	}

	public void ButtonNextLevel()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	public void ButtonRetry()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
	#endregion
}
