public interface IState
{
    void Tick(StateParams stateParams);
    void OnEnter();
    void OnExit();
}