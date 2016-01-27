using UnityEngine;
using UnityEditor;

public static class ScriptableObjectUtility
{
	public static void CreateAsset<T>() where T : ScriptableObject 
	{
		T asset = ScriptableObject.CreateInstance<T>();
		ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
	}
}
