using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	private static GameManager s_Instance = null;
	public static GameManager Instance { get { return s_Instance; } }

	private static bool s_bPaused = false;
	public static bool IsPaused { get { return s_bPaused; } }
	public bool bPlayerWon;
	public bool bGameIsOver;

	void Awake()
	{
		if (s_Instance == null)
			s_Instance = this;
		else
			Destroy(this.gameObject);

		bPlayerWon = false;
		bGameIsOver = false;
		s_bPaused = false;
		Time.timeScale = 1f;
	}

	void Update()
	{
		if (bGameIsOver == true)
			return;

		if (CheckGameEnd() == true)
		{
			EndGame();
		}
	}

	public void SetPause(bool _pause)
	{
		if (_pause)
		{
			s_bPaused = true;
			Time.timeScale = 0f;
		}
		else
		{
			s_bPaused = false;
			Time.timeScale = 1f;
		}
	}

	private bool CheckGameEnd()
	{
		if (EnemyMainFSM.Instance() == null ||
		    PlayerMain.Instance == null ||
		    EMHelper.Instance() == null)
			return false;

		if (EnemyMainFSM.Instance().CurrentStateIndex == EMState.Die)
		{
			bPlayerWon = true;
			bGameIsOver = true;
			return true;
		}
		else if (PlayerMain.Instance.IsAlive == false)
		{
			bPlayerWon = false;
			bGameIsOver = true;
			return true;
		}
		else if (EMHelper.Instance().IsEnemyWin == true)
		{
			bPlayerWon = false;
			bGameIsOver = true;
			return true;
		}

		return false;
	}

	private void EndGame()
	{
		EndGamePanel.Instance.SetEndGamePanelVisibility(true);
	}

	#region XML Readeer

	#endregion










	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
