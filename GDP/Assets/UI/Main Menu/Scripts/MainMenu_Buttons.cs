﻿using UnityEngine;
using System.Collections;

public class MainMenu_Buttons : MonoBehaviour
{
	public void Button_Play()
	{
		AudioManager.PlayMenuSoundEffect(MenuSFX.PressSelection);
		Application.LoadLevel(1);
	}
}
