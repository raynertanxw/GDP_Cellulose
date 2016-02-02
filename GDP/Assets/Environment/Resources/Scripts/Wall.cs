using UnityEngine;
using System.Collections;

// Wall.cs: Handles the spawning of the renderer and collision of the wall.
public class Wall : MonoBehaviour
{
	// Editable Fields
	[Header("Nutrients Spawn")]
	[Tooltip("The percentage of spawning when it tries to spawn")][Range(0.0f, 1.0f)]
	[SerializeField] private float fNutrientsChance = 0.5f;
	[Tooltip("The duration between each tries")]
	[SerializeField] private float fNutrientsDelay = 1.0f;
	[Tooltip("The length of the spawnable distance")]
	[SerializeField] private float fSpawnYRange = 15f;
	[Tooltip("The verticle offsent of the spawnable range")]
	[SerializeField] private float fSpawnYOffset = 2.5f;

	[Header("Wall Renderer Properties")]
	[Tooltip("The array of wall-sides")]
	[SerializeField] private WallRenderer[] array_WallSidesGO;
	[Tooltip("The array of wall-backgrounds")]
	[SerializeField] private WallRenderer[] array_WallBackgroundGO;
	[Tooltip("The travelling speed of wall-sides")]
	[SerializeField] private float fWallSidesSpeed = 1f;
	[Tooltip("The travelling speed of wall-background")]
	[SerializeField] private float fWallBackgroundSpeed = 0.8f;
	[Tooltip("The minimum RGB value for the artillery color")]
	[SerializeField] private float fMinimumArtilleryRGB = 0.3f;
	[Tooltip("The maximum RGB value for the artillery color")]
	[SerializeField] private float fMaximumArtilleryRGB = 0.8f;
	


	// Uneditable Fields
	private static Wall s_Instance = null;

	private float fUpperLimit;
	private float fLowerLimit;
	private Color colorArtillery = Color.black;
	private ParticleSystem bgParticleSystem;
	private Animate mAnimate;

	// Private Functions
	// Awake(): is called at the start of the program
	void Awake()
	{
		// Singleton
		if (s_Instance == null)
			s_Instance = this;
		else
			Destroy(this.gameObject);

		float resultColor;
		// while: Initialise a color and checks if the color is acceptable
		// Since HSV input of colors is harder to implement,
		// it converts RGB into one OVERALL value and check if is within fMinimumArtilleryRGB and fMaximumArtilleryRGB range
		do
		{
			colorArtillery = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
			resultColor = colorArtillery.r + colorArtillery.g + colorArtillery.b;

		} while (resultColor < 3f * fMinimumArtilleryRGB || resultColor > 3f * fMaximumArtilleryRGB);

		// Read level specific settings from Settings.cs
		colorArtillery = Settings.s_EnvironmentColor;
		fNutrientsChance = Settings.s_fPlayerNutrientChance;
		fWallSidesSpeed = Settings.s_fSideWallSpeed;
		fWallBackgroundSpeed = Settings.s_BackgroundSpeed;



		// Background particle system set-up 
		bgParticleSystem = transform.GetChild(2).GetComponent<ParticleSystem>();
		// Use the same color as the wall-sides and background
		bgParticleSystem.startColor = colorArtillery;
		// Since prewarm of particle systems doesn't adapt to the new color, the particle system will be simulated beforehand
		bgParticleSystem.Clear();
		bgParticleSystem.Simulate(bgParticleSystem.startLifetime);
		bgParticleSystem.Play();

		mAnimate = new Animate(transform.GetChild(0)); // Pool_WallSidesRenderer
	}

	// Start(): Use this for initialization
	void Start ()
	{
		// Fields Initialisation
		fUpperLimit = (fSpawnYRange / 2f) + fSpawnYOffset;
		fLowerLimit = -(fSpawnYRange / 2f) + fSpawnYOffset;

		// Initiate nutrients Spawning Routine
		StartCoroutine("SpawnNutrients");

		// Initiate Wall-rendering Routine
		SpawnWallSides();
		SpawnWallBackground();
		mAnimate.Idle(0.2f, 0.5f, false, false, true);
	}
	
	// SpawnNutrients(): Handles the spawning of nutrients
	IEnumerator SpawnNutrients()
	{
		while (true)
		{
			yield return new WaitForSeconds(fNutrientsDelay);
			if (UnityEngine.Random.value <= fNutrientsChance)
			{
				float xPos = 0;
				if (Random.Range(0, 2) == 1)
					xPos = 6.5f;
				else
					xPos = -6.5f;

				Vector3 spawnPos = new Vector3(xPos, Random.Range(fLowerLimit, fUpperLimit), 0);
				Nutrients.Spawn(spawnPos);
			}
		}
	}

	// SpawnWallSides(): Spawns a side-wall
	public bool SpawnWallSides()
	{
		for (int i = 0; i < array_WallSidesGO.Length; i++)
		{
			// if: The current wall-side is not enabled
			if (!array_WallSidesGO[i].enabled)
			{
				array_WallSidesGO[i].enabled = true;
				array_WallSidesGO[i].BecomesEnable();
				return true;
			}
		}
		Debug.LogWarning("Wall.SpawnWallSides(): All walls-sides are active! Perhaps add more walls to pool to fix this problem?");
		return false;
	}

	// SpawnWallBackground(): Spawns a background-wall
	public bool SpawnWallBackground()
	{
		for (int i = 0; i < array_WallBackgroundGO.Length; i++)
		{
			// if: The current wall-side is not enabled
			if (!array_WallBackgroundGO[i].enabled)
			{
				array_WallBackgroundGO[i].enabled = true;
				array_WallBackgroundGO[i].BecomesEnable();
				return true;
			}
		}
		Debug.LogWarning("Wall.SpawnWallBackground(): All walls-background are active! Perhaps add more walls to pool to fix this problem?");
		return false;
	}

	// Public Static Functions
	public static Wall Instance { get { return s_Instance; } }

	// Getter-Setter Functions
	public float NutrientsChance { get { return fNutrientsChance; } set { fNutrientsChance = value; } }
	public float NutrientsDelay { get { return fNutrientsDelay; } set { fNutrientsDelay = value; } }

	public float WallSidesSpeed { get { return fWallSidesSpeed; } }
	public float WallBackgroundSpeed { get { return fWallBackgroundSpeed; } }
	public Color ArtilleryColor { get { return colorArtillery; } }














	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
