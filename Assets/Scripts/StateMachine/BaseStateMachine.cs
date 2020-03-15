using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachine
{
    private IState _currentState;
    public IState CurrentState => _currentState;

    private List<StateTransition> _stateTransitions = new List<StateTransition>();

    public IStateParams Tick(IStateParams stateParams)
    {
        StateTransition stateTransition = CheckForTransition();
        if (stateTransition != null)
        {
            SetState(stateTransition.To);
        }
        return _currentState.Tick(stateParams);
    }

    private StateTransition CheckForTransition()
    {
        foreach (var transition in _stateTransitions)
        {
            if (transition.Condition() && transition.To != _currentState)
            {
                return transition;
            }
        }
        return null;
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