using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : State
{
    public Idle(PlayerController playerController) : base(playerController)
    {

    }

    // Enter is called once after init and before other methods.
    public override void Enter()
    {
        base.Enter();
        // Reset velocity when switching to idle so we don't get stuck in movement.
        m_Velocity = Vector3.zero;
    }

    // HandleInput is called every frame.
    public override void HandleInput()
    {
        if (!m_IsGrounded)
        {
            return;
        }
        // Get the WASD inputs and check if they were pressed.
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            // Check if Shift was pressed.
            if (Input.GetButton("Run"))
            {
                m_EventTransition.Invoke(new Run(m_PlayerController), false);
            }
            else
            {
                m_EventTransition.Invoke(new Walk(m_PlayerController), false);
            }
        }
        // Check if Spacebar was pressed.
        else if (Input.GetButton("Jump"))
        {
            m_EventTransition.Invoke(new Jump(m_PlayerController), false);
        }

        // Check if f was pressed.
        if (Input.GetButtonDown("Interact") && m_PlayerController.m_InteractObj != null)
        {
            m_EventTransition.Invoke(new Interact(m_PlayerController), false);
        }
    }

    // Idle is the default animation state and therefore has no triggers, when every other animation state is false then you must be in idle.
}
