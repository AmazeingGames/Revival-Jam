using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StateRunner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] List<State<T>> states;

    readonly Dictionary<Type, State<T>> stateToType = new();

    State<T> activeState;

    protected virtual void Awake()
    {
        states.ForEach(s => stateToType.Add(s.GetType(), s));
        SetState(states[0].GetType());
    }

    public void SetState(Type newStateType)
    {
        if (activeState != null)
            activeState.Exit();

        activeState = stateToType[newStateType];
        activeState.Enter(GetComponent<T>());
    }

    void Update()
    {
        activeState.CaptureInput();
        activeState.Update();
        activeState.ChangeState();
    }

    void FixedUpdate()
    {
        activeState.FixedUpdate();    
    }
}
