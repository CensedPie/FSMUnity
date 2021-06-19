using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : StateMachine
{
    public Camera m_CameraRef;
    public GameObject m_InteractObj;
    public GameObject m_Canvas;
    // Start is called before the first frame update
    void Start()
    {
        // We launch and enter into the first state.
        base.Transition(new Idle(this), false);
        Assert.IsNotNull(m_CameraRef);
        Assert.IsNotNull(m_Canvas);
    }

    // Update is called once per frame
    void Update()
    {
        // It's important for these methods to be in this order otherwise we will have small input lag possibly.
        m_CurrentState.HandleInput();
        m_CurrentState.Update();
    }

    public void SetInteractObj(GameObject obj)
    {
        m_InteractObj = obj;
    }

    void OnGUI()
    {
        GUI.TextField(new Rect(10, 10, 200, 100), "Position: " + gameObject.transform.position + "\nVelocity: " + m_CurrentState.GetVelocity() + "\nState: " + m_CurrentState.GetType() + "\nGrounded: " + m_CurrentState.IsGrounded() + "\nInteractObject: " + m_CurrentState.GetInteractType());
    }
}
