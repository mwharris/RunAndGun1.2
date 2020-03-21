using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachine
{
    private IState _currentState;
    public IState CurrentState => _currentState;

    private List<StateTransition> _stateTransitions = new List<StateTransition>();
    private List<StateTransition> _anyStateTransitions = new List<StateTransition>();

    public event Action<IState, IState> OnStateChanged;
    
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
        // "Any" State Transitions have priority
        foreach (var transition in _anyStateTransitions)
        {
            if (_currentState != transition.To && transition.Condition())
            {
                return transition;
            }
        }
        foreach (var transition in _stateTransitions)
        {
            if (_currentState == transition.From && transition.Condition())
            {
                return transition;
            }
        }
        return null;
    }
    
    public void SetState(IState state)
    {
        if (_currentState == state)
        {
            return;
        }
        var fromState = _currentState;
        var toState = state;
        
        _currentState?.OnExit();
        Debug.Log($"Changed from {_currentState} to {state}");
        _currentState = state;
        _currentState?.OnEnter();
        
        OnStateChanged?.Invoke(fromState, toState);
    }
    
    public void AddTransition(IState from, IState to, Func<bool> condition)
    {
        StateTransition stateTransition = new StateTransition(from, to, condition);
        _stateTransitions.Add(stateTransition);
    }
    
    public void AddAnyTransition(IState to, Func<bool> condition)
    {
        StateTransition stateTransition = new StateTransition(null, to, condition);
        _anyStateTransitions.Add(stateTransition);
    }

}