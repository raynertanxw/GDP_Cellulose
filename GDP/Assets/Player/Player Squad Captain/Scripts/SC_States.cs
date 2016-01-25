using UnityEngine;
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
    private static List<SC_IdleState> list_IdleChild = new List<SC_IdleState>();

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
        m_scFSM.Strafing();
    }

    public override void Exit()
    {
        ExecuteMethod.OnceInUpdate("SquadChildFSM.CalculateStrafingOffset", null, null);
    }
}

// SC_FindResourceState: The find resource state of the player squad's captain FSM
public class SC_FindResourceState : ISCState
{
    private Nutrients targetNutrients = null;

    // Constructor
    public SC_FindResourceState(SquadChildFSM m_SquadChildFSM)
    {
        m_scFSM = m_SquadChildFSM;
    }

    public override void Execute()
    {
        targetNutrients = m_scFSM.GetNearestResource();

        // if: There is no target
        if (targetNutrients == null)
        {
            m_scFSM.Advance(SCState.Idle);
            return;
        }
        // else if: The target nutrient has left the screen OR the player has collected it
        else if (targetNutrients.IsInPool || !targetNutrients.IsCollectable)
        {
            m_scFSM.Advance(SCState.Idle);
            return;
        }
        else
        {
            m_scFSM.RigidBody.AddForce((targetNutrients.transform.position - m_scFSM.transform.position) * Time.deltaTime * 5f, ForceMode2D.Force);
            m_scFSM.Draw(targetNutrients.transform.position);

            // if: The distance between the two bodies is less than a certain distance
            if (Vector3.Distance(targetNutrients.transform.position, m_scFSM.transform.position) < 0.5f)
            {
                m_scFSM.Advance(SCState.Dead);
                targetNutrients = null;
            }
        }
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
    // Constructor
    public SC_AttackState(SquadChildFSM m_SquadChildFSM)
    {
        m_scFSM = m_SquadChildFSM;
    }

    public override void Enter()
    {
        ExecuteMethod.OnceInUpdate("SquadChildFSM.GetNearestTargetPosition", null, null);
    }

    public override void Execute()
    {
        m_scFSM.AttackTarget();
    }

    public override void Exit()
    {

    }
}
