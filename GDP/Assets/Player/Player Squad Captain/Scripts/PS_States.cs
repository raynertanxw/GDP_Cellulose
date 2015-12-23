using UnityEngine;
using System.Collections;

// PS_IdleState: The idle state of the squad captain
public class PS_IdleState : IPSState
{
    // Uneditable Fields
    private PS_Logicaliser m_Brain;

    // Constructor
    public PS_IdleState(PlayerSquadFSM para_playerSquadFSM, PS_Logicaliser para_Brain)
    {
        m_psFSM = para_playerSquadFSM;
        m_Brain = para_Brain;
    }

    public override void Execute()
    {
        m_Brain.Think();
    }
}

// PS_AttackState: The attack state of squad captain
public class PS_AttackState : IPSState
{
    // Constructor
    public PS_AttackState(PlayerSquadFSM para_playerSquadFSM)
    {
        m_psFSM = para_playerSquadFSM;
    }
}

// PS_DefendState: The defend state of the squad captain
public class PS_DefendState : IPSState
{
    // Constructor
    public PS_DefendState(PlayerSquadFSM para_playerSquadFSM)
    {
        m_psFSM = para_playerSquadFSM;
    }
}

// PS_ProduceState: The produce state of the squad captain
public class PS_ProduceState : IPSState
{
    // Constructor
    public PS_ProduceState(PlayerSquadFSM para_playerSquadFSM)
    {
        m_psFSM = para_playerSquadFSM;
    }

    public override void Enter()
    {
        m_psFSM.EnableSpawnRoutine();
        SquadChildFSM.AdvanceSquadPercentage(SCState.Idle, SCState.Produce, 50f);

        m_psFSM.Advance(PSState.Idle);
    }
}

// PS_FindResourceState: The find resource state of the squad captain
public class PS_FindResourceState : IPSState
{
    // Constructor
    public PS_FindResourceState(PlayerSquadFSM para_playerSquadFSM)
    {
        m_psFSM = para_playerSquadFSM;
    }
}

// PS_DeadState: The dead state of the squad captain
public class PS_DeadState : IPSState
{
    // Constructor
    public PS_DeadState(PlayerSquadFSM para_playerSquadFSM)
    {
        m_psFSM = para_playerSquadFSM;
    }

    public override void Enter()
    {
        m_psFSM.m_SpriteRenderer.enabled = false;
        m_psFSM.m_Collider.enabled = false;
        m_psFSM.bIsAlive = false;

        m_psFSM.transform.position = Constants.s_farfarAwayVector;
    }

    public override void Exit()
    {
        m_psFSM.m_SpriteRenderer.enabled = true;
        m_psFSM.m_Collider.enabled = true;
        m_psFSM.bIsAlive = true;
    }
}