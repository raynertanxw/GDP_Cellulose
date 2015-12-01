using UnityEngine;
using System.Collections;

public class PlayerMain : MonoBehaviour
{
	public int m_nHealth = 100;

	public static PlayerMain s_Instance;

	void Awake()
	{
		if (PlayerMain.s_Instance == null)
		{
			PlayerMain.s_Instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
