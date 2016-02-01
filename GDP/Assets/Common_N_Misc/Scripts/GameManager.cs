using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	private static GameManager s_Instance = null;
	public static GameManager Instance { get { return s_Instance; } }

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

	private bool CheckGameEnd()
	{
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
