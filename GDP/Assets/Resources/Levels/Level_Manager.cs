using UnityEngine;
using System.Collections;

public class Level_Manager
{
	public static void LoadLevel(int _level)
	{
		string path = "Levels/LEVEL_" + _level.ToString();
		LevelTemplate levelData = Resources.Load(path) as LevelTemplate;
		SetSettings(levelData);
	}

	public static void LoadTutorial()
	{
		string path = "Levels/LEVEL_TUTORIAL";
		LevelTemplate tutorialData = Resources.Load(path) as LevelTemplate;
		SetSettings(tutorialData);
	}

	private static void SetSettings(LevelTemplate _data)
	{
		Settings.s_nPlayerInitialHealth = _data.PlayerInitialHealth;
		Settings.s_nPlayerInitialResourceCount = _data.PlayerInitialResourceCount;
	}
}
