﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// SC_States.cs: Stores all the states for Squad Child Finite State Machine

// SC_DeadState: The death state of the player squad's captain FSM
public class SC_DeadState : ISCState
{
	// Constructor
	public SC_DeadState(SquadChildFSM m_SquadChildFSM)
	{
		m_scFSM = m_SquadChildFSM;
	}

	public override void Enter()
	{
		m_scFSM.m_SpriteRenderer.enabled = false;
		m_scFSM.m_Collider.enabled = false;
		m_scFSM.bIsAlive = false;

		m_scFSM.transform.position = Constants.s_farfarAwayVector;
	}

	public override void Exit()
	{
		m_scFSM.m_SpriteRenderer.enabled = true;
		m_scFSM.m_Collider.enabled = true;
		m_scFSM.bIsAlive = true;
	}
}

// SC_IdleState: The idle state of the player squad's captain FSM
public class SC_IdleState : ISCState
{
	// Static Fields
	private static List<SC_IdleState> list_IdleChild;

	private static float s_fIdleDistance = PlayerSquadFSM.Instance.IdleDistance;
	private static float s_fIdleRigidity = PlayerSquadFSM.Instance.IdleRigidity;
	private static float s_fIdleRadius = PlayerSquadFSM.Instance.IdleRadius;
	private static float s_fIdleMaximumVelocity = PlayerSquadFSM.Instance.IdleMaximumVelocity;

	// Uneditable Fields
	private float fAngularPosition = -1f;
	private float fAngularVelocity = -1f;

	// Constructor
	public SC_IdleState(SquadChildFSM m_SquadChildFSM)
	{
		list_IdleChild = new List<SC_IdleState>(); 
		m_scFSM = m_SquadChildFSM;
	}

	// State Execution:
	// Enter(): When the squad child enters this state
	public override void Enter()
	{
		list_IdleChild.Add(this);

		// Spawns in a random direction
		if (fAngularPosition == -1f)
			fAngularPosition = UnityEngine.Random.value * 360.0f;
		if (fAngularVelocity == -1f)
			fAngularVelocity = UnityEngine.Random.value * s_fIdleMaximumVelocity - (s_fIdleMaximumVelocity / 2f);

	}

	// Execute(): When the squad child is in this state. Runs once every frame on every instance
	public override void Execute()
	{

		// Calculates the velocity for all children
		ExecuteMethod.OnceInUpdate("SC_IdleState.CalculateVelocity", null, null);

		// toTargetVector: The vector from its tranform position to the angular position
		Vector3 toTargetVector = PlayerSquadFSM.Instance.transform.position + Quaternion.Euler(0.0f, 0.0f, fAngularPosition) * Vector3.up * s_fIdleRadius - m_scFSM.transform.position;
		if (toTargetVector.magnitude > s_fIdleRigidity)
			m_scFSM.m_RigidBody.AddForce(toTargetVector);
		m_scFSM.m_RigidBody.velocity = Vector3.ClampMagnitude(m_scFSM.m_RigidBody.velocity, Mathf.Max(s_fIdleRigidity, toTargetVector.magnitude));

		fAngularPosition += fAngularVelocity * Time.deltaTime;
		// if, else if: Clamping values to be within 0 to 360f
		if (fAngularPosition > 360.0f)
			fAngularPosition -= 360.0f;
		else if (fAngularPosition < 0.0f)
			fAngularPosition += 360.0f;
	}

	// Exit(): When the squad child exits this state
	public override void Exit()
	{
		list_IdleChild.Remove(this);
	}

	// Public Static Functions:
	// CalculateVelocity(): Re-calculates all the velocity of all squad children that is in idling state
	public static bool CalculateVelocity()
	{
		// for, for: The for loops will check if i with j and j with i whether they are close to each other. Checks are onmi-directional
		for (int i = 0; i < list_IdleChild.Count; i++)
		{
			for (int j = 0; j < list_IdleChild.Count; j++)
			{
				// if: i and j is not the same squad child
				if (i != j)
				{
					// distanceBetweenChild: Initialise a value between 0 and 180 between the two angles
					float distanceBetweenChild = Mathf.DeltaAngle(list_IdleChild[j].fAngularPosition, list_IdleChild[i].fAngularPosition);
					// if: The distance between two children is less than the desired space, space them apart
					if (distanceBetweenChild < s_fIdleDistance && distanceBetweenChild > 0.0f)
					{
						// j child moves to the left, i child moves to the right, Mathf.Max() is used to prevent infinity;
						list_IdleChild[j].fAngularVelocity -= s_fIdleDistance / Mathf.Max(1f, distanceBetweenChild);
						list_IdleChild[i].fAngularVelocity += s_fIdleDistance / Mathf.Max(1f, distanceBetweenChild);

						// Clamp the velocity of the children
						if (list_IdleChild[j].fAngularVelocity > 0.0f)
							list_IdleChild[j].fAngularVelocity = Mathf.Min(s_fIdleMaximumVelocity, list_IdleChild[j].fAngularVelocity);
						else
							list_IdleChild[j].fAngularVelocity = Mathf.Max(-s_fIdleMaximumVelocity, list_IdleChild[j].fAngularVelocity);

						if (list_IdleChild[i].fAngularVelocity > 0.0f)
							list_IdleChild[i].fAngularVelocity = Mathf.Min(s_fIdleMaximumVelocity, list_IdleChild[i].fAngularVelocity);
						else
							list_IdleChild[i].fAngularVelocity = Mathf.Max(-s_fIdleMaximumVelocity, list_IdleChild[i].fAngularVelocity);

						//Debug.Log("Idle distance between " + i + " and " + j + ": " + distanceBetweenChild + " = " + list_IdleChild[i].fAngularPosition + " + " + list_IdleChild[j].fAngularPosition);
					}
				}
			}
		}
		return true;
	}
}

// SC_ProduceState: The produce state of the player squad's captain FSM
public class SC_ProduceState : ISCState
{
	// Constructor
	public SC_ProduceState(SquadChildFSM m_SquadChildFSM)
	{
		m_scFSM = m_SquadChildFSM;
	}

	public override void Enter()
	{
		ExecuteMethod.OnceInUpdate("SquadChildFSM.CalculateStrafingOffset", null, null);
	}

	public override void Execute()
	{
		ExecuteMethod.OnceInUpdate("SquadChildFSM.UpdateLandmineList", null, null);

		m_scFSM.Strafing();

		// if: There is more landmine than the production child count
		if (SquadChildFSM.ListLandmine.Count >= SquadChildFSM.StateCount(SCState.Produce))
		{
			for (int i = 0; i < SquadChildFSM.ListLandmine.Count; i++)
			{
				if (Vector3.Distance(SquadChildFSM.ListLandmine[i].transform.position, m_scFSM.transform.position) < 2f)
				{
					m_scFSM.Advance(SCState.Avoid);
					return;
				}
			}
		}
		// else if: There is lesser landmine state BUT not 0
		else if (SquadChildFSM.ListLandmine.Count != 0)
		{
			for (int i = 0; i < SquadChildFSM.ListLandmine.Count; i++)
			{
				if (Vector3.Distance(SquadChildFSM.ListLandmine[i].transform.position, m_scFSM.transform.position) < 2f)
				{
					m_scFSM.Advance(SCState.Attack);
					return;
				}
			}
		}
	}

	public override void Exit()
	{
		ExecuteMethod.OnceInUpdate("SquadChildFSM.CalculateStrafingOffset", null, null);
	}
}

// SC_FindResourceState: The find resource state of the player squad's captain FSM
public class SC_FindResourceState : ISCState
{
	private static Vector3 vCenterPosition = Vector3.zero;

	private Nutrients targetNutrients = null;

	// Constructor
	public SC_FindResourceState(SquadChildFSM m_SquadChildFSM)
	{
		m_scFSM = m_SquadChildFSM;
	}

	public override void Enter()
	{
		m_scFSM.m_SpriteRenderer.color = new Color(1.0f, 1.0f, 0f);
	}

	public override void Execute()
	{
		targetNutrients = m_scFSM.GetNearestResource();

		// if: The target nutrient is too close to the wall, disable the target
		if (targetNutrients != null)
			if (Mathf.Abs(targetNutrients.transform.position.x) >= 4f)
				targetNutrients = null;

		// Seperation Factor
		// vSeperationVector: The final vector used to space away from all the other nearby cells
		Vector3 vSeperationVector = Vector3.zero;
		Collider2D[] array_nearbyCollider = Physics2D.OverlapCircleAll(m_scFSM.transform.position, 0.5f, Constants.s_onlySquadChildLayer);

		if (array_nearbyCollider.Length > 0)
		{
			for (int i = 0; i < array_nearbyCollider.Length; i++)
			{
				// if: The current gameObject is itself
				if (array_nearbyCollider[i] == m_scFSM.gameObject)
					continue;

				// vDirectionVector: The vector that the current squad child should be travelling in to space away from the current neighbour
				Vector3 vDirectionVector = (m_scFSM.transform.position - array_nearbyCollider[i].transform.position);

				// if: Somehow this is triggered, which is very often...
				if (vDirectionVector.magnitude > 0)
					vSeperationVector += vDirectionVector.normalized / vDirectionVector.sqrMagnitude;
			}

			// Average the the final vector to use for spacing away
			vSeperationVector = vSeperationVector / array_nearbyCollider.Length;
			vSeperationVector = Vector3.ClampMagnitude(vSeperationVector, 7f);

		}

		// Attraction Factor
		ExecuteMethod.OnceInUpdate("SC_FindResourceState.RecalculateCenter", null, null);
		Vector3 vAttractionVector = vCenterPosition - m_scFSM.transform.position;
		vAttractionVector *= 10f;

		// Final Velocity Vector
		// toTargetVector: The vector between the target nutrients and the current squad child cells
		Vector3 toTargetVector;
		// if: There is no target nutrient, the find nutrients group will idle infront of squad group
		if (targetNutrients == null)
			toTargetVector = PlayerSquadFSM.Instance.transform.position + new Vector3(0f, 2f, 0f) - m_scFSM.transform.position;
		else
			toTargetVector = targetNutrients.transform.position - m_scFSM.transform.position;
		toTargetVector *= 10f;

		if (targetNutrients != null)
			SquadChildFSM.Circle(targetNutrients.transform.position);

		//Debug.Log("toTargetVector: " + toTargetVector.magnitude + ", vSeperationVector: " + vSeperationVector.magnitude + ", vAttractionVector: " + vAttractionVector.magnitude);

		// Apply vector to velocity
		/* Quick Vector Summary:
		 * toTargetVector -> The vector between the nutrients and the 'FindResource' group
		 * vSeperationVector -> The personal space vector 
		 * vAttractionVector -> The 'stay as a group' vector
		 */
		m_scFSM.RigidBody.AddForce(toTargetVector + vSeperationVector + vAttractionVector * Time.deltaTime * 100f, ForceMode2D.Force);
		m_scFSM.RigidBody.velocity = Vector3.ClampMagnitude(m_scFSM.RigidBody.velocity, 3f);

		// if: The distance between the two bodies is less than a certain distance
		if (targetNutrients != null)
		{
			if (Vector3.Distance(targetNutrients.transform.position, m_scFSM.transform.position) < 0.5f)
			{
				// if: The current squad child is added to the nutrients
				if (targetNutrients.AddSquadChildCount())
				{
					m_scFSM.Advance(SCState.Dead);
					targetNutrients = null;
				}
			}
		}
	}

	public override void Exit()
	{
		m_scFSM.m_SpriteRenderer.color = Color.white;
	}

	// Public Static Function
	// RecalculateCenter(): Recalculates the center position for all find resource children
	public static void RecalculateCenter()
	{
		vCenterPosition = Vector3.zero;

		float fChildCount = 0;
		for (int i = 0; i < SquadChildFSM.SquadChildArray.Length; i++)
		{
			if (SquadChildFSM.SquadChildArray[i].EnumState == SCState.FindResource)
			{
				vCenterPosition += SquadChildFSM.SquadChildArray[i].transform.position;
				fChildCount++;
			}
		}

		// if: Prevention of inifinity error
		if (fChildCount > 0)
			vCenterPosition = vCenterPosition / fChildCount;
	}
}

// SC_DefendState: The defend state of the player squad's captain FSM
public class SC_DefendState : ISCState
{
	// Constructor
	public SC_DefendState(SquadChildFSM m_SquadChildFSM)
	{
		m_scFSM = m_SquadChildFSM;
	}

	public override void Enter()
	{
		Debug.Log(SquadChildFSM.CalculateDefenceSheildOffset());
	}

	public override void Execute()
	{
		m_scFSM.DefenceSheild();
	}

	public override void Exit()
	{
		ExecuteMethod.OnceInUpdate("SquadChildFSM.CalculateDefenceSheildOffset", null, null);
	}
}

// SC_AttackState: The attack state of the player squad's captain FSM
public class SC_AttackState : ISCState
{
	public bool isColorChanged = false;
	// Constructor
	public SC_AttackState(SquadChildFSM m_SquadChildFSM)
	{
		m_scFSM = m_SquadChildFSM;
	}

	public override void Enter()
	{
		m_scFSM.mAnimate.ExpandContract(1000f, 2000, 1.5f, true, 0.0f);
		isColorChanged = false;
		
		AudioManager.PlaySquadSoundEffect(SquadSFX.ChildAttack);
	}

	public override void Execute()
	{
		if (m_scFSM.attackTarget != null && !isColorChanged)
		{
			m_scFSM.m_SpriteRenderer.color = new Color(1f, 0.12f, 0.12f);
			isColorChanged = true;
		}

		ExecuteMethod.OnceInUpdate("SquadChildFSM.GetNearestTargetPosition", null, null);
		m_scFSM.AttackTarget();
	}

	public override void Exit()
	{
		m_scFSM.mAnimate.StopExpandContract(false);
		m_scFSM.m_SpriteRenderer.color = Color.white;
		isColorChanged = false;
	}
}

// SC_AvoidState: The avoid state of the player squad's captain
public class SC_AvoidState : ISCState
{
	public SC_AvoidState(SquadChildFSM para_SquadChildFSM)
	{
		m_scFSM = para_SquadChildFSM;
	}

	public override void Execute()
	{
		// Re-initialisation of variables
		ExecuteMethod.OnceInUpdate("SquadChildFSM.UpdateLandmineList", null, null);
		Vector3 finalMovement = Vector3.zero; // finalMovement: The final movement in which will be used for that current frame

		// if: There is no landmine, return the squad child to idle
		if (SquadChildFSM.ListLandmine.Count == 0)
		{
			m_scFSM.Advance(SCState.Idle);
			return;
		}

		// vectorApart: The distance between the current squad child cells and the current enemy child cells
		Vector3 smallestVectorApart = Vector3.one * 3f;
		Vector3 currentVectorApart;
		// for: Check every landmine and determines the closest landmine to avoid
		for (int i = 0; i < SquadChildFSM.ListLandmine.Count; i++)
		{
			currentVectorApart = m_scFSM.transform.position - SquadChildFSM.ListLandmine[i].transform.position;

			// if: The current distance between the enemy and child cells is smaller than the current smallest
			//     then use this new vector as the smallest
			if (currentVectorApart.magnitude < smallestVectorApart.magnitude)
				smallestVectorApart = currentVectorApart;
		}
		// finalMovement: The final vector to travel to avoid other cells
		finalMovement = smallestVectorApart.normalized * (1f - smallestVectorApart.magnitude / 3f);

		//Debug.Log(m_scFSM.gameObject.name + "::Rigibody->velocity(): " + m_scFSM.RigidBody.velocity + ", finalMovment: " + finalMovement);

		m_scFSM.RigidBody.AddForce(finalMovement * 10.0f);
		m_scFSM.RigidBody.velocity = Vector3.ClampMagnitude(m_scFSM.RigidBody.velocity, finalMovement.magnitude * 0.5f);
	}
}