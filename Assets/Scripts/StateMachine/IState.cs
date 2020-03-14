public interface IState
{
    // Handle all of the logic for this state 
    IStateParams Tick(IStateParams stateParams);
    // Handle any setup that needs to be done before this state starts processing
    void OnEnter();
    // Handle any teardown that needs to be done before transitioning to another state
    void OnExit();
}