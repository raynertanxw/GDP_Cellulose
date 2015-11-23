using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
    // Editable Fields
    [Header("Nutrients Spawn")]
    [Tooltip("The percentage of spawning when it tries to spawn")][Range(0.0f, 1.0f)]
    [SerializeField] private float fNutrientsChance = 0.5f;
    [Tooltip("The duration between each tries")]
    [SerializeField] private float fNutrientsDelay = 1.0f;
    [Tooltip("The nutrient gameObject to spawn")]
	[SerializeField] private GameObject nutrientGO;

    // Uneditable Fields
    private float minY = -4f;
    private float maxY = 4f;

	// Start(): Use this for initialization
	void Start ()
	{
		StartCoroutine(SpawnNutrients());	
	}
	
    // SpawnNutrients(): Handles the spawning of nutrients
	IEnumerator SpawnNutrients()
	{
		while (true)
		{
			yield return new WaitForSeconds(fNutrientsChance);
			if (Random.Range(0, 10) >= fNutrientsDelay * 10f)
			{
				float xPos = 0;
				if (Random.Range(0, 2) == 1)
					xPos = 3.2f;
				else
					xPos = -3.2f;

				Vector3 spawnPos = new Vector3(xPos, Random.Range(minY, maxY), 0);
				Instantiate(nutrientGO, spawnPos, Quaternion.identity);
			}
		}
	}

    // Getter-Setter Functions
    public float NutrientsChance { get { return fNutrientsChance; } set { fNutrientsChance = value; } }
    public float NutrientsDelay { get { return fNutrientsDelay; } set { fNutrientsDelay = value; } }
}
