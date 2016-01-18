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
    private static List<SC_IdleState> list_IdleChild = new List<SC_IdleState>();

    private static float s_fIdleDistance = PlayerSquadFSM.Instance.IdleDistance;
    private static float s_fIdleRigidity = PlayerSquadFSM.Instance.IdleRigidity;
    private static float s_fIdleRadius = PlayerSquadFSM.Instance.IdleRadius;
    private static float s_fIdleMaximumVelocity = PlayerSquadFSM.Instance.IdleMaximumVelocity;

    private float fAngularPosition = 0f;
    private float fAngularVelocity = 0f;

    // Constructor
    public SC_IdleState(SquadChildFSM m_SquadChildFSM)
    {
        m_scFSM = m_SquadChildFSM;
    }

    // Private Functions:
    //private static bool RecalculateAveragePosition()
    //{
    //    if (SquadChildFSM.StateCount(SCState.Idle) == 0)
    //        return false;

    //    averagePosition = Vector3.zero;                                     // averagePosition: Temporary used as the total vector of all idle child
    //    SquadChildFSM[] array_SquadChild = SquadChildFSM.SquadChildArray;   // array_SquadChild: The array of squad child
    //    int nIdleCount = 0;                                                 // nIdleCount: The number of squad child in idle state
    //    for (int i = 0; i < array_SquadChild.Length; i++)
    //    {
    //        if (array_SquadChild[i].EnumState == SCState.Idle)
    //        {
    //            averagePosition += array_SquadChild[i].transform.position;
    //            nIdleCount++;
    //        }
    //    }

    //    averagePosition = averagePosition / (float)nIdleCount;
    //    return true;
    //}

    public static bool CalculateVelocity()
    {

        for (int i = 0; i < list_IdleChild.Count - 1; i++)
        {
            for (int j = i + 1; j < list_IdleChild.Count; j++)
            {
                // distanceBetweenChild: Initialise a value between 0 and 180 between the two angles
                float distanceBetweenChild = Mathf.Abs(Mathf.DeltaAngle(list_IdleChild[j].fAngularPosition, list_IdleChild[i].fAngularPosition));
                // if: The distance between two children is less than the desired space, space them apart
                if (distanceBetweenChild < s_fIdleDistance)
                {
                    // j child moves to the left, i child moves to the right, Mathf.Max() is used to prevent infinity;
                    list_IdleChild[j].fAngularVelocity += 1.0f - (Mathf.Max(1f, distanceBetweenChild) / s_fIdleDistance) * s_fIdleDistance;
                    list_IdleChild[i].fAngularVelocity -= (1.0f - (Mathf.Max(1f, distanceBetweenChild) / s_fIdleDistance) * s_fIdleDistance);

                    // Clamp the velocity of the children
                    if (list_IdleChild[j].fAngularVelocity > 0.0f)
                        list_IdleChild[j].fAngularVelocity = Mathf.Min(s_fIdleMaximumVelocity, list_IdleChild[j].fAngularVelocity);
                    else
                        list_IdleChild[j].fAngularVelocity = Mathf.Max(-s_fIdleMaximumVelocity, list_IdleChild[j].fAngularVelocity);

                    if (list_IdleChild[i].fAngularVelocity > 0.0f)
                        list_IdleChild[i].fAngularVelocity = Mathf.Min(s_fIdleMaximumVelocity, list_IdleChild[i].fAngularVelocity);
                    else
                        list_IdleChild[i].fAngularVelocity = Mathf.Max(-s_fIdleMaximumVelocity, list_IdleChild[i].fAngularVelocity);

                    //Debug.Log("Idle distance between " + i + " and " + j + ": " + distanceBetweenChild);
                }
            }
        }
        return true;
    }

    // State Execution:
    public override void Enter()
    {
        list_IdleChild.Add(this);

        // Spawns in a random direction
        fAngularPosition = UnityEngine.Random.value * 360.0f;
        fAngularVelocity = UnityEngine.Random.value * s_fIdleMaximumVelocity - (s_fIdleMaximumVelocity / 2f);
    }

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

        m_scFSM.gizmoo(toTargetVector + m_scFSM.transform.position);

        //// if: the distance to target is smaller than 0.1f -> then sets a new targetPosition
        //if ((targetPosition - m_scFSM.transform.position).sqrMagnitude < 0.1f)
        //{
        //    // Idling Squad cells got too far from the captain
        //    if ((PlayerSquadFSM.Instance.transform.position - m_scFSM.transform.position).magnitude > 1.0f)
        //    {
        //        targetPosition = PlayerSquadFSM.Instance.transform.position - m_scFSM.transform.position;
        //        // if: The cell is further away from the squad captain in the x-direction than the y-direction
        //        if (targetPosition.x > targetPosition.y)
        //            targetPosition = new Vector3(0f, targetPosition.y, 0f);
        //        else
        //            targetPosition = new Vector3(targetPosition.x, 0f, 0f);
        //        targetPosition = m_scFSM.transform.position + targetPosition;
        //    }
        //    else
        //    {
        //        bool isX = Random.value >= 0.5f;
        //        if (isX)
        //            targetPosition = m_scFSM.transform.position + new Vector3(0.5f, 0f, 0f);
        //        else
        //            targetPosition = m_scFSM.transform.position + new Vector3(0f, 0.5f, 0f);
        //    }
        //}
        //else
        //{
        //    m_scFSM.transform.position = Vector3.Lerp(m_scFSM.transform.position, targetPosition, Time.deltaTime);
        //}
    }

    public override void Exit()
    {
        list_IdleChild.Remove(this);
        m_scFSM.gizmoo(Vector3.zero);
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
    // Constructor
    public SC_FindResourceState(SquadChildFSM m_SquadChildFSM)
    {
        m_scFSM = m_SquadChildFSM;
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
        m_scFSM.attackTarget = null;
    }
}
