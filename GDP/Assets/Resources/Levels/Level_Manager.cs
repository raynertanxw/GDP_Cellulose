using UnityEngine;
using System.Collections;

public class Level_Manager
{
	public static void LoadLevel(int _level)
	{
		string path = "Levels/LEVEL_" + _level.ToString();
		LevelTemplate levelData = Resources.Load(path) as LevelTemplate;

		Settings.s_nPlayerInitialHealth = levelData.PlayerInitialHealth;
		Settings.s_nPlayerInitialResourceCount = levelData.PlayerInitialResourceCount;
	}
}
