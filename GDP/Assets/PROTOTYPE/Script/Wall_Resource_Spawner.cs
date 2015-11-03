using UnityEngine;
using System.Collections;

public class Wall_Resource_Spawner : MonoBehaviour
{
	float minY = -4f;
	float maxY = 4f;

	[SerializeField]
	private GameObject resourcePrefab;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine(spawnResources());	
	}
	
	IEnumerator spawnResources()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			if (Random.Range(0, 10) >= 7)
			{
				float xPos = 0;
				if (Random.Range(0, 2) == 1)
					xPos = 3.2f;
				else
					xPos = -3.2f;

				Vector3 spawnPos = new Vector3(xPos, Random.Range(minY, maxY), 0);
				Instantiate(resourcePrefab, spawnPos, Quaternion.identity);
			}
		}
	}
}
