using UnityEngine;
using System;
using System.Collections;

public class AnimateHandler : MonoBehaviour 
{
	// Static Fields
	private static Animate[] s_arrayExpandContract;     // s_arrayExpandContract: All Animate.cs that is in this going to run an update loop

	// Ediatable Fields
	[Header("Cache Size")]
	[Tooltip("The maximum amount of expand-contract animation it can handle at once")]
	[SerializeField] private int nExpandContractCache = 10;

	// Private Functions
	// Start(): Use this for initialization
	void Start () 
	{
		// Definition of expand-contract array
		s_arrayExpandContract = new Animate[nExpandContractCache];
	}
	
	// Update(): is called once per frame
	void Update () 
	{
		// for: Expand-Contract Checking Sequence
		for (int i = 0; i < s_arrayExpandContract.Length; i++)
		{
			if (s_arrayExpandContract[i] != null)
			{
				// if: The current Animate.cs no longer needs to run
				if (!s_arrayExpandContract[i].UpdateExpandContract(Time.deltaTime))
					s_arrayExpandContract[i] = null;

			}
		}
	}

	// Public Static Functions
	// ActivateExpandContract(): Pushes an Animate.cs into update sequence
	public static bool ActivateExpandContract(Animate _mAnimate)
	{
		for (int i = 0; i < s_arrayExpandContract.Length; i++)
		{
			if (s_arrayExpandContract[i] == null)
			{
				s_arrayExpandContract[i] = _mAnimate;
				return true;
			}
		}
		Debug.LogWarning("AnimateHandler.ActivateExpandContract(): Cache have reached its maximum limit, consider creating a bigger cache?");
		return false;
	}
}
