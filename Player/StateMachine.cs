using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State m_CurrentState;

    // When invoking the transition you have to give 2 parameters. The state you want to switch to (as a new object using the new keyword) and if you want to initialize the state or not.
    // Initializing the state means giving it a direction and velocity. This is usefull for states where you want to keep the previous velocity like jump.
    public void Transition(State state, bool init)
    {
        // Declare our temp velocity and direction for initializing.
        Vector3 velocity = Vector3.zero;
        Vector3 direction = Vector3.zero;
        // If we want to initialize the next state then we will send it the current states speed.
        if (init)
        {
            velocity = m_CurrentState.GetVelocity();
            direction = m_CurrentState.GetDirection();
        }
        // The first time we launch, the idle state is created through the transition. Therefore we don't have a current state to exit the first time we launch the game.
        if (m_CurrentState != null)
        {
            m_CurrentState.Exit();
        }
        // Update the current state to be the next state we wanted.
        m_CurrentState = state;
        // If we wanted to initialize then we pass the velocity and direction to the state. This is called first, then Enter below, then it will go into HandleInput and Update loop.
        if (init)
        {
            m_CurrentState.Init(velocity, direction);
        }
        m_CurrentState.Enter();
    }
}