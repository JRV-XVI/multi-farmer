using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    public State<T> CurrentState { get; private set; }

    public void ChangeState(State<T> newState)
    {
        if (CurrentState != null)
            CurrentState.OnExitState();

        CurrentState = newState;

        if (CurrentState != null)
            CurrentState.OnEnterState();
    }

    public void Update()
    {
        if (CurrentState != null)
            CurrentState.OnStayState();
    }


    // Redirecciones de triggers
    public void OnTriggerEnter(Collider other)
    {
        if (CurrentState != null)
            CurrentState.OnTriggerEnter(other);
    }

    public void OnTriggerStay(Collider other)
    {
        if (CurrentState != null)
            CurrentState.OnTriggerStay(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (CurrentState != null)
            CurrentState.OnTriggerExit(other);
    }
}
