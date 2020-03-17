public class Idle : IState
{
    private readonly Player _player;
    
    public bool IsIdle { get; private set; }
    
    public Idle(Player player)
    {
        _player = player;
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        // Update our stateParams velocity
        var stateParamsVelocity = stateParams.Velocity;
        stateParamsVelocity.x = 0;
        stateParamsVelocity.z = 0;
        stateParams.Velocity = stateParamsVelocity;
        return stateParams;
    }

    public bool CheckIdle()
    {
        var noHorizontal = !PlayerInput.Instance.HorizontalHeld;
        var noVertical = !PlayerInput.Instance.VerticalHeld;
        return noHorizontal && noVertical;
    }
    
    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

}