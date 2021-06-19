using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : State
{
    public Walk(PlayerController playerController) : base(playerController)
    {

    }

    // Enter is called once after init and before other methods.
    public override void Enter()
    {
        base.Enter();
        //m_AnimComponent.SetBool("Walk", true);
    }

    // HandleInput is called every frame.
    public override void HandleInput()
    {
        if (!m_IsGrounded)
        {
            m_EventTransition.Invoke(new Fall(m_PlayerController), true);
            //return;
        }
        // Get inputs WASD and if none are pressed go to idle.
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            m_EventTransition.Invoke(new Idle(m_PlayerController), false);
        }
        // If Shift is pressed go to run.
        else if (Input.GetButton("Run"))
        {
            m_EventTransition.Invoke(new Run(m_PlayerController), true);
        }
        // If Spacebar is pressed go to jump.
        else if (Input.GetButton("Jump"))
        {
            m_EventTransition.Invoke(new Jump(m_PlayerController), true);
        }
        else if(Input.GetButton("Interact") && m_PlayerController.m_InteractObj != null)
        {
            m_EventTransition.Invoke(new Interact(m_PlayerController), false);
        }
    }

    // Update is called every frame and is not the Unity update function but a normal C# method called on a Unity update function. Check PlayerController.cs.
    public override void Update()
    {
        base.Update();

        // The direction to move the character in based on inputs. This is absolute values aka World space which is why we have camera code below.
        m_Direction.x = Input.GetAxisRaw("Horizontal");
        m_Direction.z = Input.GetAxisRaw("Vertical");

        // Normalize it so we don't run diagonally faster.
        m_Direction = m_Direction.normalized;

        // Should be self explanitory.
        m_Velocity.x = m_Direction.x * m_WalkSpeed;
        m_Velocity.z = m_Direction.z * m_WalkSpeed;

        // The region below with camera is to allow movement in the direction the camera is facing. This makes the GTA style movement where W is towards camera facing, A is 90 degrees left of camera facing, etc.
        // Get the camera forward and right vectors to use in movement below.
        Vector3 camForward = m_PlayerController.m_CameraRef.transform.forward;
        Vector3 camRight = m_PlayerController.m_CameraRef.transform.right;

        // Set the y to 0 so we don't take into account up or down.
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        // Code will rotate player to cam depending on the direction keys pressed.
        #region Rotate Player To Cam
        if (m_Direction.z > 0)
        {
            if (m_Direction.x > 0)
            {
                m_RotateOffset = 45f;

            }
            else if (m_Direction.x < 0)
            {
                m_RotateOffset = -45f;
            }
            else
            {
                m_RotateOffset = 0f;
            }
        }
        else if (m_Direction.z < 0)
        {
            if (m_Direction.x > 0)
            {
                m_RotateOffset = 135f;
            }
            else if (m_Direction.x < 0)
            {
                m_RotateOffset = -135f;
            }
            else
            {
                m_RotateOffset = 180f;
            }
        }
        else
        {
            if (m_Direction.x > 0)
            {
                m_RotateOffset = 90f;
            }
            else if (m_Direction.x < 0)
            {
                m_RotateOffset = -90f;
            }
        }
        m_CharController.transform.rotation = Quaternion.Lerp(m_CharController.transform.rotation, Quaternion.Euler(0, m_PlayerController.m_CameraRef.transform.eulerAngles.y + m_RotateOffset, 0), 0.1f);
        #endregion

        // Move the character relative to the camera facing. Z is forward and back, X is left and right.
        m_CharController.Move((camForward * m_Velocity.z + camRight * m_Velocity.x) * Time.deltaTime);
    }

    // Exit is called once just before the state is changed.
    public override void Exit()
    {
        base.Exit();
        // Exit the walk animation state.
        //m_AnimComponent.SetBool("Walk", false);
    }
}
