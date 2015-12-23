using UnityEngine;
using System.Collections;

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
    // Uneditable Fields
    private Vector3 targetPosition;     // targetPosition: The target position that the cell is travelling towards

    // Constructor
    public SC_IdleState(SquadChildFSM m_SquadChildFSM)
    {
        m_scFSM = m_SquadChildFSM;
    }

    // State Execution:
    public override void Enter()
    {
        targetPosition = m_scFSM.transform.position;
    }

    public override void Execute()
    {
        // if: the distance to target is smaller than 0.1f -> then sets a new targetPosition
        if ((targetPosition - m_scFSM.transform.position).sqrMagnitude < 0.1f)
        {
            // Idling Squad cells got too far from the captain
            if ((PlayerSquadFSM.Instance.transform.position - m_scFSM.transform.position).magnitude > 1.0f)
            {
                targetPosition = PlayerSquadFSM.Instance.transform.position - m_scFSM.transform.position;
                // if: The cell is further away from the squad captain in the x-direction than the y-direction
                if (targetPosition.x > targetPosition.y)
                    targetPosition = Vector3.Cross(targetPosition, Vector3.right);
                else
                    targetPosition = Vector3.Cross(targetPosition, Vector3.up);
                targetPosition = m_scFSM.transform.position + targetPosition;
            }
            else
            {
                bool isX = Random.value >= 0.5f;
                if (isX)
                    targetPosition = m_scFSM.transform.position + new Vector3(0.5f, 0f, 0f);
                else
                    targetPosition = m_scFSM.transform.position + new Vector3(0f, 0.5f, 0f);
            }
        }
        else
        {
            m_scFSM.transform.position = Vector3.Lerp(m_scFSM.transform.position, targetPosition, Time.deltaTime);
        }
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
        SquadChildFSM.CalculateStrafingOffset();
    }

    public override void Execute()
    {

        m_scFSM.Strafing();
    }

    public override void Exit()
    {
        SquadChildFSM.CalculateStrafingOffset();
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
}

// SC_AttackState: The attack state of the player squad's captain FSM
public class SC_AttackState : ISCState
{
    // Constructor
    public SC_AttackState(SquadChildFSM m_SquadChildFSM)
    {
        m_scFSM = m_SquadChildFSM;
    }
}
