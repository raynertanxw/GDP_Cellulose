using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
	public static void LoadScene(int _sceneId)
	{
		Application.LoadLevel(2);

		// Call All Reset Statics
		PlayerChildFSM.ResetStatics();
		PlayerMain.ResetStatics();
		player_control.ResetStatics();
		GameManager.ResetStatics();
		EndGamePanel.ResetStatics();

		Wall.ResetStatics();
		WallRenderer.ResetStatics();
		Nutrients.ResetStatics();

		ECPoolManager.ResetStatics();

		Application.LoadLevel(_sceneId);
	}

	public static void LoadScene(string _sceneName)
	{
		Application.LoadLevel(2);
		
		// Call All Reset Statics
		PlayerChildFSM.ResetStatics();
		PlayerMain.ResetStatics();
		player_control.ResetStatics();
		GameManager.ResetStatics();
		EndGamePanel.ResetStatics();
		
		ECPoolManager.ResetStatics();
		
		Application.LoadLevel(_sceneName);
	}
}
