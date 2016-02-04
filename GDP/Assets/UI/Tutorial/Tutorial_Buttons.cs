using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial_Buttons : MonoBehaviour
{
	public void Button_BackToMainMenu()
	{
		SceneManager.LoadScene(0);
		if(AudioManager.Instance != null){AudioManager.Instance.PlayMenuSoundEffect(MenuSFX.PressSelection);}
	}
}
