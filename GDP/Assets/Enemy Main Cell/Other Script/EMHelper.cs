using UnityEngine;
using System.Collections;

[RequireComponent (typeof (EnemyMainFSM))]
[RequireComponent (typeof (EMController))]
[RequireComponent (typeof (Renderer))]
[RequireComponent (typeof (CircleCollider2D))]
public class EMHelper : MonoBehaviour 
{
	// Instance of the class
	private static EMHelper instance;
	// Singleton
	public static EMHelper Instance()
	{
		return instance;
	}

	EnemyMainFSM m_EMFSM;

	public ECPoolManager ECPool;

	public static float leftLimit;
	public static float rightLimit;
	public static float topLimit;
	public static float bottomLimit;

	#region Properties of the enemy main cell
	private Vector2 position;
	public Vector2 Position { get { return position; } set { position = value; } }
	private float fMinToMaxYRatio;
	public float MinToMaxYRatio { get { return fMinToMaxYRatio; } }
	private float fRadius;
	public float Radius { get { return fRadius; } }
	private float width;
	#endregion

	[SerializeField]
	private bool bIsEnemyVisible;
	public bool IsEnemyVisible { get { return bIsEnemyVisible; } }
	[SerializeField]
	private bool bIsColliderEnabled;
	public bool IsColliderEnabled { get { return bIsColliderEnabled; } }
	[SerializeField]
	private bool bIsEnemyWin;
	public bool IsEnemyWin { get { return bIsEnemyWin; } }
	// Production status
	[SerializeField]
	private bool bCanSpawn; 
	public bool CanSpawn { get { return bCanSpawn; } set { bCanSpawn = value; } }
	// Avilability of commanding child cells
	private bool bCanAddDefend;
	public bool CanAddDefend { get { return bCanAddDefend; } set { bCanAddDefend = value; } }
	private bool bCanAddAttack;
	public bool CanAddAttack { get { return bCanAddAttack; } set { bCanAddAttack = value; } }
	private bool bCanAddLandmine;
	public bool CanAddLandmine { get { return bCanAddLandmine; } set { bCanAddLandmine = value; } }


	void Start () 
	{
		if (instance == null)
			instance = this;

		// GetComponent
		m_EMFSM = GetComponent<EnemyMainFSM> ();
		width = GetComponent<CircleCollider2D> ().bounds.size.x;
		fRadius = GetComponent<CircleCollider2D> ().bounds.size.x;
		position = transform.position;

		// Find gameObject
		ECPool = GameObject.Find("Enemy Child Cell Pool").GetComponent<ECPoolManager>();

		// Initialise status
		bIsEnemyVisible = true;
		bIsEnemyWin = false;
		bCanSpawn = true;
		// Able to command child cells to any state by default
		bCanAddDefend = true;
		bCanAddAttack = true;
		bCanAddLandmine = true;
	}
	
	void Update () 
	{
		// Remove destroyed items in enemy child list
		if (EnemyMainFSM.Instance () != null)
			EnemyMainFSM.Instance().ECList.RemoveAll(item => item.CurrentStateEnum == ECState.Dead);
		//Recalculate available enemy child cells
		m_EMFSM.AvailableChildNum = m_EMFSM.ECList.Count;
		// Check whether the player loses
		LoseCheck ();
		// Check if the enemy main cell is visible
		VisibilityCheck ();
	}

	void FixedUpdate ()
	{
		// Update Radius
		if (fRadius != GetComponent<CircleCollider2D> ().bounds.size.x)
			fRadius = GetComponent<CircleCollider2D> ().bounds.size.x;
		// Update the ratio of y distance
		if (fMinToMaxYRatio != (transform.position.y - Settings.s_fEnemyMainMinY) / (Settings.s_fEnemyMainMaxY - Settings.s_fEnemyMainMinY))
			fMinToMaxYRatio = (transform.position.y - Settings.s_fEnemyMainMinY) / (Settings.s_fEnemyMainMaxY - Settings.s_fEnemyMainMinY);
		// Update camera border
		CameraLimit ();
		// Make sure the enemy main cell do not go outside the screen
		PositionLimit ();
		// Update width of enemy main cell
		WidthUpdate();
		// Update enemy main position
		position = transform.position;
		// Prevent having more than 100 child cells
		ChildCellsLimit ();
	}

	#region Functions for pausing commanding child cells
	// Pause to command child cells to Attack state
	public IEnumerator PauseAddAttack (float fTime)
	{
		bCanAddAttack = false;
		yield return new WaitForSeconds (fTime);
		bCanAddAttack = true;
	}
	// Pause to command child cells to Defend state
	public IEnumerator PauseAddDefend (float fTime)
	{
		bCanAddDefend = false;
		yield return new WaitForSeconds (fTime);
		bCanAddDefend = true;
	}
	// Pause to command child cells to Landmine state
	public IEnumerator PauseAddLandmine (float fTime)
	{
		bCanAddLandmine = false;
		yield return new WaitForSeconds (fTime);
		bCanAddLandmine = true;
	}
	#endregion

	// Camera limit update
	void CameraLimit ()
	{
		EMHelper.leftLimit = Camera.main.ViewportToWorldPoint (Vector3.zero).x;
		EMHelper.rightLimit = Camera.main.ViewportToWorldPoint (Vector3.right).x;
		EMHelper.bottomLimit = Camera.main.ViewportToWorldPoint (Vector3.zero).y;
		EMHelper.topLimit = Camera.main.ViewportToWorldPoint (Vector3.up).y;
	}
	#region Position
    // Make sure the enemy main cell do not go outside the screen
    void PositionLimit ()
	{
		if (transform.position.x <= EMHelper.leftLimit + width || transform.position.x >= EMHelper.rightLimit - width)
		{
			// Make sure the enemy main cell is cannot go out of the camera
			if (transform.position.x < EMHelper.leftLimit + width)
				transform.position = new Vector2(EMHelper.leftLimit + width, transform.position.y);
			else if (transform.position.x > EMHelper.rightLimit - width)
				transform.position = new Vector2(EMHelper.rightLimit - width, transform.position.y);

			EMController.Instance ().ChangeDirection ();
		}

		if (transform.position.y < Settings.s_fEnemyMainMinY)
			transform.position = new Vector2 (transform.position.x, Settings.s_fEnemyMainMinY);
	}
    // Update width of enemy main cell
	void WidthUpdate ()
	{
		// Get the width without considering expand animation
		if (width != EMAnimation.Instance().InitialScale.x * Mathf.Sqrt (Mathf.Sqrt (Mathf.Sqrt (m_EMFSM.Health))))
			width = EMAnimation.Instance().InitialScale.x * Mathf.Sqrt (Mathf.Sqrt (Mathf.Sqrt (m_EMFSM.Health)));
	}
	#endregion
	// Checking whether the enemy main cell goes out of the screen or runs out of health 
	// Make it invisible if it does
	private void LoseCheck ()
	{
		if (transform.position.y - EMHelper.Instance ().Radius / 2f > EMHelper.topLimit) 
		{
			Visibility (false);
			bIsEnemyWin = true;
		}
	}
	// Check if the enemy main cell is visible and update collider status according to it
	private void VisibilityCheck ()
	{
		if (GetComponent<Renderer> ().enabled) 
		{
			bIsEnemyVisible = true;
		}
		else 
		{
			bIsEnemyVisible = false;
		}
	}
	// Prevent having more than 100 child cells
	private void ChildCellsLimit ()
	{
		if (m_EMFSM.AvailableChildNum >= 100)
			bCanSpawn = false;
	}
	// Make the Enemy Main Cell invisible
	public void Visibility (bool isVisible)
	{
		if (isVisible) 
		{
			if (!GetComponent<Renderer> ().enabled)
				GetComponent<Renderer> ().enabled = true;
		}
		else if (!isVisible) 
		{
			if (GetComponent<Renderer> ().enabled)
				GetComponent<Renderer> ().enabled = false;
		}
	}
	// Used to handle everything happens when spawns a child cell
	public IEnumerator ProduceChild ()
	{
		if (bCanSpawn) 
		{
			bCanSpawn = false;

			// Calling the Animate class for spawn animation
			Animate mAnimate;
			mAnimate = new Animate (this.transform);
			mAnimate.ExpandContract (0.1f, 1, 1.1f);
			
			EMController.Instance().ReduceNutrient ();
			// Randomize the interval time between spawns of child cells in terms of current difficulty
			yield return new WaitForSeconds (
				UnityEngine.Random.Range (
				1.0f / EMDifficulty.Instance().CurrentDiff, 
				1.5f / EMDifficulty.Instance().CurrentDiff)
				);
			
			if (m_EMFSM.AvailableChildNum < 100)
				bCanSpawn = true;
		}
	}

	#region Coroutine functions
	// Things needed when produce child cells
	public void StartProduceChild ()
	{
		StartCoroutine (ProduceChild ());
	}
	// Disable transition for fTime
	public void StartPauseTransition (float fTime)
	{
		StartCoroutine (EMTransition.Instance().TransitionAvailability (fTime));
	}
	// Stop commanding child cells to Attack state for fTime
	public void StartPauseAddAttack (float fTime)
	{
		StartCoroutine (PauseAddAttack (fTime));
	}
	// Stop commanding child cells to Defend state for fTime
	public void StartPauseAddDefend (float fTime)
	{
		StartCoroutine (PauseAddDefend (fTime));
	}
	// Stop commanding child cells to Landmine state for fTime
	public void StartPauseAddLandmine (float fTime)
	{
		StartCoroutine (PauseAddLandmine (fTime));
	}
	#endregion

	#region Math
	// Returns value raised to power
	public float Pow (float value, float power)
	{
		return Mathf.Pow (value, power);
	}
	// Returns square root of value
	public float Sqrt (float value)
	{
		return Mathf.Sqrt (value);
	}
	// Returns the absolute value of value
	public float Abs (float value)
	{
		return Mathf.Abs (value);
	}
	#endregion
}