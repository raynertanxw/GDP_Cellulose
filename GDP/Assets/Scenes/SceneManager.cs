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

		PlayerSquadFSM.ResetStatics();
		SquadChildFSM.ResetStatics();
		SC_IdleState.ResetStatics();

		Wall.ResetStatics();
		WallRenderer.ResetStatics();
		Nutrients.ResetStatics();

		ECPoolManager.ResetStatics();
		ECIdleState.ResetStatics();
		DirectionDatabase.ResetStatics();
		FormationDatabase.ResetStatics();
		PathQuery.ResetStatics();
		PointDatabase.ResetStatics();
		PositionQuery.ResetStatics();
		ECTracker.ResetStatics();

		EnemyMainFSM.ResetStatics();

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

		PlayerSquadFSM.ResetStatics();
		SquadChildFSM.ResetStatics();
		SC_IdleState.ResetStatics();
		
		Wall.ResetStatics();
		WallRenderer.ResetStatics();
		Nutrients.ResetStatics();
		
		ECPoolManager.ResetStatics();
		ECIdleState.ResetStatics();
		DirectionDatabase.ResetStatics();
		FormationDatabase.ResetStatics();
		PathQuery.ResetStatics();
		PointDatabase.ResetStatics();
		PositionQuery.ResetStatics();
		ECTracker.ResetStatics();
		
		EnemyMainFSM.ResetStatics();
		
		Application.LoadLevel(_sceneName);
	}
}
