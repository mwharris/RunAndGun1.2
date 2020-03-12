using UnityEngine;

public class BaseStateMachine
{
    private IState _currentState;
    public IState CurrentState => _currentState;

    // TODO: Pass in an interface instead of StateParams object?...
    // TODO: Return the interface as well?...
    public void Tick(StateParams stateParams)
    {
        _currentState.Tick(stateParams);
    }

    public void SetState(IState state)
    {
        if (_currentState == state)
        {
            return;
        }
        
        _currentState?.OnExit();
        _currentState = state;
        Debug.Log($"Changed state to {state}");
        state?.OnEnter();
    }
}