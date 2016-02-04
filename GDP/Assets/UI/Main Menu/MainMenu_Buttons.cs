﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenu_Buttons : MonoBehaviour
{
	
	[Range(0.01f, 0.1f)]
	public float scrollSpeed = 0.1f;
	public float minY, maxY;
	private Transform cameraTransform;

	private bool shouldSnapUp, shouldSnapDown, shouldSnapBack, isSnapping;
	private MainMenuPosition _menuPosition;
	private float snapTolerance = 125.0f;
	private float snapSpeed = 35.0f;

	void Awake()
	{
		cameraTransform = GameObject.Find("Main Camera").transform;

		shouldSnapUp = false;
		shouldSnapDown = false;
		shouldSnapBack = false;
		isSnapping = false;
		_menuPosition = MainMenuPosition.Center;
	}

	void Update()
	{
		if (shouldSnapUp)
		{
			cameraTransform.position += Vector3.up * snapSpeed * Time.deltaTime;
			switch(_menuPosition)
			{
			case MainMenuPosition.Bottom:
				if (cameraTransform.position.y > 0f)
				{
					cameraTransform.position = new Vector3(cameraTransform.position.x, 0f, cameraTransform.position.z);
					_menuPosition = MainMenuPosition.Center;
					shouldSnapUp = false;
					isSnapping = false;
				}
				break;
			case MainMenuPosition.Center:
				if (cameraTransform.position.y > maxY)
				{
					cameraTransform.position = new Vector3(cameraTransform.position.x, maxY, cameraTransform.position.z);
					_menuPosition = MainMenuPosition.Top;
					shouldSnapUp = false;
					isSnapping = false;
				}
				break;
			}
		}

		if (shouldSnapDown)
		{
			cameraTransform.position -= Vector3.up * snapSpeed * Time.deltaTime;
			switch(_menuPosition)
			{
			case MainMenuPosition.Top:
				if (cameraTransform.position.y < 0f)
				{
					cameraTransform.position = new Vector3(cameraTransform.position.x, 0f, cameraTransform.position.z);
					_menuPosition = MainMenuPosition.Center;
					shouldSnapDown = false;
					isSnapping = false;
				}
				break;
			case MainMenuPosition.Center:
				if (cameraTransform.position.y < minY)
				{
					cameraTransform.position = new Vector3(cameraTransform.position.x, minY, cameraTransform.position.z);
					_menuPosition = MainMenuPosition.Bottom;
					shouldSnapDown = false;
					isSnapping = false;
				}
				break;
			}
		}

		if (shouldSnapBack)
		{
			switch(_menuPosition)
			{
			case MainMenuPosition.Top:
				_menuPosition = MainMenuPosition.Center;
				shouldSnapUp = true;
				isSnapping = true;
				break;
			case MainMenuPosition.Center:
				if (cameraTransform.position.y > 0)
				{
					_menuPosition = MainMenuPosition.Top;
					shouldSnapDown = true;
					isSnapping = true;
				}
				else
				{
					_menuPosition = MainMenuPosition.Bottom;
					shouldSnapUp = true;
					isSnapping = true;
				}
				break;
			case MainMenuPosition.Bottom:
				_menuPosition = MainMenuPosition.Center;
				shouldSnapDown = true;
				isSnapping = true;
				break;
			}

			shouldSnapBack = false;
		}
	}

	public void Button_Level(int _level)
	{
		AudioManager.Instance.PlayMenuSoundEffect(MenuSFX.PressSelection);

		Level_Manager.LoadLevel(_level);
		SceneManager.LoadScene(1);
	}

	public void Button_Tutorial()
	{
		AudioManager.Instance.PlayMenuSoundEffect(MenuSFX.PressSelection);

		Level_Manager.LoadTutorial();
		SceneManager.LoadScene(3);
	}


	#region Animation Helper Functions
	private void SnapUp()
	{
		if (_menuPosition == MainMenuPosition.Top)
			return;

		shouldSnapUp = true;
		shouldSnapDown = false;
		shouldSnapBack = false;
		isSnapping = true;
	}

	private void SnapDown()
	{
		if (_menuPosition == MainMenuPosition.Bottom)
			return;

		shouldSnapDown = true;
		shouldSnapUp = false;
		shouldSnapBack = false;
		isSnapping = true;
	}

	private void SnapBack()
	{
		shouldSnapDown = false;
		shouldSnapUp = false;
		shouldSnapBack = true;
	}
	#endregion


	#region Event Trigger functions
	public void Drag(BaseEventData _data)
	{
		if (isSnapping)
			return;

		PointerEventData _pointerData = _data as PointerEventData;

		cameraTransform.position += new Vector3(0f, _pointerData.delta.y * -scrollSpeed, 0f);

		if (cameraTransform.position.y < minY)
			cameraTransform.position = new Vector3(cameraTransform.position.x, minY, cameraTransform.position.z);
		else if (cameraTransform.position.y > maxY)
			cameraTransform.position = new Vector3(cameraTransform.position.x, maxY, cameraTransform.position.z);
	}

	public void EndDrag(BaseEventData _data)
	{
		if (isSnapping)
			return;

		PointerEventData _pointerData = _data as PointerEventData;

		// Inverse scrolling.
		float dragDeltaY = _pointerData.position.y - _pointerData.pressPosition.y;
		if (dragDeltaY < -snapTolerance)
		{
			SnapUp();
		}
		else if (dragDeltaY > snapTolerance)
		{
			SnapDown();
		}
		else
		{
			SnapBack();
		}
	}
	#endregion
}