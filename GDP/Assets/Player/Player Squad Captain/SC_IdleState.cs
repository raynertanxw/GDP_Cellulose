using UnityEngine;
using System.Collections;

// SC_IdleState.cs: The idle state of the player squad's captain FSM
public class SC_IdleState : ISCState 
{
    // Uneditable Fields
    private Vector3 targetPosition;     // targetPosition: The target position that the cell is travelling towards

    // Constructor
    public SC_IdleState(PlayerSquadFSM m_PlayerSquadFSM)
    {
        m_scFSM = m_PlayerSquadFSM;
    }

    // State Execution:
    public override void Enter()
    {
        targetPosition = m_scFSM.transform.position;
    }

    public override void Execute()
    {
        // if: The number of squad child cells is less than 30 <- Production Check
        if (PlayerSquadFSM.AliveCount() < 4f)
        {
            m_scFSM.Advance(SCState.Produce);
            return;
        }

        // if: the distance to target is smaller than 0.1f -> then sets a new targetPosition
        if ((targetPosition - m_scFSM.transform.position).sqrMagnitude < 0.1f)
        {
            // Idling Squad cells got too far from the captain
            if ((SquadCaptain.Instance.transform.position - m_scFSM.transform.position).magnitude > 1.0f)
            {
                targetPosition = SquadCaptain.Instance.transform.position - m_scFSM.transform.position;
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
