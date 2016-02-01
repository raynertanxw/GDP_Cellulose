using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenu_Buttons : MonoBehaviour
{
	
	[Range(0.01f, 0.1f)]
	public float scrollSpeed = 0.1f;
	public float minY, maxY;
	private Transform cameraTransform;

	void Awake()
	{
		cameraTransform = GameObject.Find("Main Camera").transform;
	}

	public void Button_Level(int _level)
	{
		AudioManager.PlayMenuSoundEffect(MenuSFX.PressSelection);

		Level_Manager.LoadLevel(_level);
		SceneManager.LoadScene(1);
	}

	public void Button_Tutorial()
	{
		AudioManager.PlayMenuSoundEffect(MenuSFX.PressSelection);

		Level_Manager.LoadTutorial();
		SceneManager.LoadScene(3);
	}


	#region Event Trigger functions
	public void Drag(BaseEventData _data)
	{
		PointerEventData _pointerData = _data as PointerEventData;

		cameraTransform.position += new Vector3(0f, _pointerData.delta.y * -scrollSpeed, 0f);

		if (cameraTransform.position.y < minY)
			cameraTransform.position = new Vector3(cameraTransform.position.x, minY, cameraTransform.position.z);
		else if (cameraTransform.position.y > maxY)
			cameraTransform.position = new Vector3(cameraTransform.position.x, maxY, cameraTransform.position.z);
	}
	#endregion
}
