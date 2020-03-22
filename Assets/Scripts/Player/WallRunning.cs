public class WallRunning : IState
{
    private readonly float _gravity;

    public WallRunning(float defaultGravity)
    {
        _gravity = defaultGravity;
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        var stateParamsVelocity = stateParams.Velocity;
        stateParams.Velocity = stateParamsVelocity;
        return SetGravity(stateParams);
    }

    private IStateParams SetGravity(IStateParams stateParams)
    {
        if (stateParams.Velocity.y < 0f)
        {
            stateParams.GravityOverride = _gravity / 4f;
        }
        else
        {
            stateParams.GravityOverride = _gravity / 1.5f;
        }
        return stateParams;
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }
}