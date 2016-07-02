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

	private bool shouldSnapUp, shouldSnapDown, shouldSnapBack, isSnapping;
	private MainMenuPosition _menuPosition;
	private float snapTolerance = 125.0f;
	private float snapSpeed = 30.0f;
	private float slowDownFactor;
	private float minSlowDownFactor = 0.6f;

	private CanvasGroup swipeTextCanvasGroup;
	private float alphaMultiplier = 0.1f;

	private Transform titleTransform;
	private float titlePosMultiplier = 0.464f;	// 5.8 / 12.5
	private Animate titleAnimate;

	public Sprite[] alienBodySprites;
	private Image alienBodyImage;
	private SpriteRenderer[] backgroundImage;
	public Color[] backgroundColors;
	public ParticleSystem[] particleSystems;

	void Awake()
	{
        Screen.SetResolution(360, 640, false);

		cameraTransform = GameObject.Find("Main Camera").transform;

		shouldSnapUp = false;
		shouldSnapDown = false;
		shouldSnapBack = false;
		isSnapping = false;
		_menuPosition = MainMenuPosition.Center;

		swipeTextCanvasGroup = transform.GetChild(4).GetComponent<CanvasGroup>();
		titleTransform = transform.GetChild(1);
		titleAnimate = new Animate(titleTransform);

		alienBodyImage = transform.GetChild(3).GetChild(0).GetComponent<Image>();
		alienBodyImage.sprite = alienBodySprites[Random.Range(0, alienBodySprites.Length)];
		backgroundImage = new SpriteRenderer[5];
		backgroundImage[0] = transform.GetChild(5).GetComponent<SpriteRenderer>();
		backgroundImage[1] = transform.GetChild(6).GetComponent<SpriteRenderer>();
		backgroundImage[2] = transform.GetChild(7).GetComponent<SpriteRenderer>();
		backgroundImage[3] = transform.GetChild(8).GetComponent<SpriteRenderer>();
		backgroundImage[4] = transform.GetChild(9).GetComponent<SpriteRenderer>();

		int randColor = Random.Range(0, backgroundColors.Length);
		for (int i = 0; i < backgroundImage.Length; i++)
		{
			backgroundImage[i].color = backgroundColors[randColor];
		}

		for (int i = 0; i < particleSystems.Length; i++)
		{
			particleSystems[i].startColor = backgroundColors[randColor];
			particleSystems[i].Clear();
			particleSystems[i].Simulate(particleSystems[i].startLifetime);
			particleSystems[i].Play();
		}
	}

	void Start()
	{
		titleAnimate.IdleRotation(-3.0f, 3.0f, 2f, 4f, true, false);
	}

	void Update()
	{
		if (shouldSnapUp)
		{
			cameraTransform.position += Vector3.up * snapSpeed * Time.deltaTime * slowDownFactor;
			if (slowDownFactor > minSlowDownFactor)
				slowDownFactor *= 0.925f;
			else
				slowDownFactor = minSlowDownFactor;

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

		swipeTextCanvasGroup.alpha = 1f - Mathf.Abs(cameraTransform.position.y) * alphaMultiplier;
		titleTransform.position = new Vector3(0f, cameraTransform.position.y * titlePosMultiplier);
		if (titleAnimate.IsExpandContract == false)
			titleAnimate.ExpandContract(2.0f, 1, 1.1f);
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

		Level_Manager.LoadLevel(1);
		//Level_Manager.LoadTutorial();
		SceneManager.LoadScene("Tutorial");
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
		slowDownFactor = 1.0f;
	}

	private void SnapDown()
	{
		if (_menuPosition == MainMenuPosition.Bottom)
			return;

		shouldSnapDown = true;
		shouldSnapUp = false;
		shouldSnapBack = false;
		isSnapping = true;
		slowDownFactor = 1.0f;
	}

	private void SnapBack()
	{
		shouldSnapDown = false;
		shouldSnapUp = false;
		shouldSnapBack = true;
		slowDownFactor = 1.0f;
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
