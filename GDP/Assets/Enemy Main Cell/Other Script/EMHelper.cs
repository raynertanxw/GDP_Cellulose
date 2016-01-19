using UnityEngine;
using System.Collections;

[RequireComponent (typeof (EnemyMainFSM))]
[RequireComponent (typeof (EMController))]
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

	// Properties of the enemy main cell
	private Vector2 position;
	public Vector2 Position { get { return position; } set { position = value; } }
	private float fRadius;
	public float Radius { get { return fRadius; } }
	private float width;

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

		// Able to command child cells to any state by default
		bCanAddDefend = true;
		bCanAddAttack = true;
		bCanAddLandmine = true;
	}
	
	void Update () 
	{
		// Remove destroyed items in enemy child list
		EnemyMainFSM.Instance().ECList.RemoveAll(item => item.CurrentStateEnum == ECState.Dead);
		//Recalculate available enemy child cells
		m_EMFSM.AvailableChildNum = m_EMFSM.ECList.Count;
	}

	void FixedUpdate ()
	{
		// Update Radius
		if (fRadius != GetComponent<CircleCollider2D> ().bounds.size.x)
			fRadius = GetComponent<CircleCollider2D> ().bounds.size.x;
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
	}
    // Update width of enemy main cell
	void WidthUpdate ()
	{
		// Get the width without considering expand animation
		if (width != EMAnimation.Instance().InitialScale.x * Mathf.Sqrt (Mathf.Sqrt (Mathf.Sqrt (m_EMFSM.Health))))
			width = EMAnimation.Instance().InitialScale.x * Mathf.Sqrt (Mathf.Sqrt (Mathf.Sqrt (m_EMFSM.Health)));
	}
	// Prevent having more than 100 child cells
	void ChildCellsLimit ()
	{
		if (m_EMFSM.AvailableChildNum >= 100)
			m_EMFSM.CanSpawn = false;
	}

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