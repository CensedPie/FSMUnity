using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : State
{
    protected Vector3 m_CamForward;
    protected Vector3 m_CamRight;
    protected float m_SlopeLimitRef;
    protected float m_StepOffsetRef;

    public Fall(PlayerController playerController) : base(playerController)
    {

    }

    public override void Enter()
    {
        base.Enter();

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

        //m_AnimComponent.SetBool("Fall", true);
    }

    public override void HandleInput()
    {

        base.HandleInput();

        if (m_IsGrounded && m_Velocity.y <= 0)
        {
            if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
            {
                m_EventTransition.Invoke(new Run(m_PlayerController), true);
            }
            else
            {
                m_EventTransition.Invoke(new Idle(m_PlayerController), true);
            }
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //check when doing jumping, test out functionality. Cam code turns char in turn of direction you jump until you hit the ground. Might need to take direction from jump state, maybe other variables, 

        // Get the direction we are jumping in.
        Vector3 heading = m_CamForward * m_Velocity.z + m_CamRight * m_Velocity.x;
        // Rotate our character to that direction to face him in the direction of the jump.
        // This is for when you are facing one direction but jump in a different direction like jumping to the side. The character will be rotated from his facing to the jump direction facing.
        m_CharController.transform.rotation = Quaternion.Lerp(m_CharController.transform.rotation, Quaternion.Euler(0, Mathf.Atan2(heading.x, heading.z) * Mathf.Rad2Deg, 0), 0.1f);

        // Move the character in the direction we were heading.
        m_CharController.Move((m_CamForward * m_Velocity.z + m_CamRight * m_Velocity.x) * Time.deltaTime);
    }

    public override void Exit()
    {
        base.Exit();

        // Maybe grab m_jumping and set to false here instead of in jump state?Review

        m_InAir = false;
        m_CharController.slopeLimit = m_SlopeLimitRef;
        m_CharController.stepOffset = m_StepOffsetRef;
        //m_AnimComponent.SetBool("Fall", false);
    }
}
