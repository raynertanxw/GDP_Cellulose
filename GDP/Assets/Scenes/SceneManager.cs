using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
	public static void LoadScene(int _sceneId)
	{
		Application.LoadLevel(2);

		// Call All Reset Statics

		Application.LoadLevel(_sceneId);
	}

	public static void LoadScene(string _sceneName)
	{
		Application.LoadLevel(2);
		
		// Call All Reset Statics
		
		Application.LoadLevel(_sceneName);
	}
}
