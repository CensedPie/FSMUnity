using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : State
{
    protected Vector3 m_CamForward;
    protected Vector3 m_CamRight;
    protected float m_SlopeLimitRef;
    protected float m_StepOffsetRef;
    public Jump(PlayerController playerController) : base(playerController)
    {

    }

    // Enter is called once after init and before other methods.
    public override void Enter()
    {
        base.Enter();
        // Set jumping to true so that we know to apply real gravity. If we are not jumping but are grounded we apply the small gravity for stairs and bumps.
        m_InAir = true;
        m_Velocity.y += m_JumpForce;

        // Get camera reference so we can keep our facing while jumping.
        m_CamForward = m_PlayerController.m_CameraRef.transform.forward;
        m_CamRight = m_PlayerController.m_CameraRef.transform.right;

        // Set camera to y to 0 to ignore up-down.
        m_CamForward.y = 0;
        m_CamRight.y = 0;
        m_CamForward = m_CamForward.normalized;
        m_CamRight = m_CamRight.normalized;

        // Remember slope limit and step offset of character controller.
        m_SlopeLimitRef = m_CharController.slopeLimit;
        m_StepOffsetRef = m_CharController.stepOffset;

        // Set character controllers slope limit and step offset to 0 to avoid bugs with jumping up slopes and steps.
        m_CharController.slopeLimit = 0f;
        m_CharController.stepOffset = 0f;

        // Prepare the start of the jump animation. JumpBlend is the blend between jump up and fall animations. Jump is a trigger for the jump animation state.
        // JumpBlend TO BE REPLACED BY FALLING STATE.
        //m_AnimComponent.SetFloat("JumpBlend", 0);
        //m_AnimComponent.SetBool("Jump", true);
    }

    // HandleInput is called every frame.
    public override void HandleInput()
    {
        base.HandleInput();

        // If not grounded and y velocity is <= 0 then set to falling state
        if (!m_IsGrounded && m_Velocity.y <= 0)
        {
            m_EventTransition.Invoke(new Fall(m_PlayerController), true);
        }

    }

    // Update is called every frame and is not the Unity update function but a normal C# method called on a Unity update function. Check PlayerController.cs.
    public override void Update()
    {
        base.Update();

        // Get the direction we are jumping in.
        Vector3 heading = m_CamForward * m_Velocity.z + m_CamRight * m_Velocity.x;
        // Rotate our character to that direction to face him in the direction of the jump.
        // This is for when you are facing one direction but jump in a different direction like jumping to the side. The character will be rotated from his facing to the jump direction facing.
        m_CharController.transform.rotation = Quaternion.Lerp(m_CharController.transform.rotation, Quaternion.Euler(0, Mathf.Atan2(heading.x, heading.z) * Mathf.Rad2Deg, 0), 0.1f);

        // Move the character in the direction we were heading.
        m_CharController.Move((m_CamForward * m_Velocity.z + m_CamRight * m_Velocity.x) * Time.deltaTime);
        // Blend between jump up animation and falling animation.
        // JumpBlend TO BE REPLACED BY FALLING STATE.
        //m_AnimComponent.SetFloat("JumpBlend", Mathf.Clamp(m_Velocity.y / 20f, 0f, 1f));
    }

    // Exit is called once just before the state is changed.
    public override void Exit()
    {
        base.Exit();
        // Set jumping to false to allow gravity over small bumps.
        //m_InAir = false;
        // Reset slope and step offset to allow climbing slopes and stairs.
        m_CharController.slopeLimit = m_SlopeLimitRef;
        m_CharController.stepOffset = m_StepOffsetRef;
        // Exit the jump animation state.
        //m_AnimComponent.SetBool("Jump", false);
    }
}
