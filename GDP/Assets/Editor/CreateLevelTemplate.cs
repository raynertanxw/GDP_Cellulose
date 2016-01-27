using UnityEngine;
using UnityEditor;

public class CreateLevelTemplate
{
	[MenuItem("Assets/Create/GDP/Level Template")]
	public static void CreateAsset()
	{
		ScriptableObjectUtility.CreateAsset<LevelTemplate>();
	}
}
