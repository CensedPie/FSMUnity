using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected PlayerController m_PlayerController;
    protected CharacterController m_CharController;
    protected delegate void TransitionEvent(State state, bool init);
    protected TransitionEvent m_EventTransition;
    protected InteractTypes.InteractType m_InteractType = InteractTypes.InteractType.None;
    //protected Animator m_AnimComponent;
    #region Movement
    protected float m_WalkSpeed = 5f;
    protected float m_RunSpeed = 10f;
    protected float m_JumpForce = 40f;
    protected float m_RotateOffset = 0f;
    protected Vector3 m_Direction = Vector3.zero;
    protected Vector3 m_Velocity = Vector3.zero;
    #endregion
    #region Gravity
    protected float m_Gravity = 0.5f;
    protected bool m_IsGrounded = false;
    protected bool m_InAir = false;
    #endregion

    protected State(PlayerController playerController)
    {
        //Pass the player controller object to get the event and have access to other components.
        m_PlayerController = playerController;
        m_EventTransition += playerController.Transition;
        m_CharController = playerController.gameObject.GetComponent<CharacterController>();
        //m_AnimComponent = playerController.gameObject.GetComponent<Animator>();
    }

    // Init is called once before all other methods and after the constructor.
    public virtual void Init(Vector3 velocity, Vector3 direction)
    {
        // This is to keep the speed and direction when needed (example: jumping in a direction while running).
        m_Velocity.x = velocity.x;
        m_Velocity.z = velocity.z;
        m_Direction = direction;
    }

    // Enter is called once after init and before other methods.
    public virtual void Enter()
    {

    }

    // HandleInput is called every frame.
    public virtual void HandleInput()
    {

    }

    // Update is called every frame and is not the Unity update function but a normal C# method called on a Unity update function. Check PlayerController.cs.
    public virtual void Update()
    {
        RaycastHit hitInfo;
        // Check if the capsule collider is touching the ground or if the raycast hits the ground. (Capsule collider is buggy and sometimes reports false but we are colliding with the ground. So we do a redundant check with spherecast).
        m_IsGrounded = m_CharController.isGrounded || Physics.SphereCast(m_CharController.transform.position + m_CharController.center, 0.5f, Vector3.down, out hitInfo, 0.6f);
        if (!m_IsGrounded)
        {
            m_Velocity.y -= m_Gravity;
            // Don't want to jump faster if there is a physics bug or fall faster.
            m_Velocity.y = Mathf.Clamp(m_Velocity.y, -20f, 20f);
        }
        else
        {
            // This is for small bumps where we are still touching the ground but we can move up and down a bit (like stairs).
            if (!m_InAir)
            {
                m_Velocity.y = -m_Gravity;
            }
        }
        // Move the character but only for gravity. Other movements are handled in their own states.
        m_CharController.Move(new Vector3(0, m_Velocity.y, 0) * Time.deltaTime);
    }

    // Exit is called once just before the state is changed.
    public virtual void Exit()
    {

    }

    // For debug purposes.
    public bool IsGrounded()
    {
        return m_IsGrounded;
    }
    public Vector3 GetVelocity()
    {
        return m_Velocity;
    }

    public Vector3 GetDirection()
    {
        return m_Direction;
    }

    public void SetInteractType(InteractTypes.InteractType type)
    {
        m_InteractType = type;
    }
    
    public InteractTypes.InteractType GetInteractType()
    {
        return m_InteractType;
    }
}
