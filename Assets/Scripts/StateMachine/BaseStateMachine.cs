using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachine
{
    private IState _currentState;
    public IState CurrentState => _currentState;

    private List<StateTransition> _stateTransitions;

    public IStateParams Tick(IStateParams stateParams)
    {
        _currentState.Tick(stateParams);
        return stateParams;
    }

    public void SetState(IState state)
    {
        _currentState?.OnExit();
        _currentState = state;
        Debug.Log($"Changed to state {state}");
        _currentState?.OnEnter();
    }

    public void AddStateTransition(StateTransition stateTransition)
    {
        _stateTransitions.Add(stateTransition);
    }
}