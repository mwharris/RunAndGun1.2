using UnityEngine;

public class WallRunning : IState
{
    private readonly Player _player;
    private readonly float _gravity;

    public WallRunning(Player player, float defaultGravity)
    {
        _player = player;
        _gravity = defaultGravity;
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        var stateParamsVelocity = stateParams.Velocity;
        var wallRunHitInfo = stateParams.WallRunHitInfo;
        
        var wallRunDirection = Vector3.ProjectOnPlane(stateParamsVelocity, wallRunHitInfo.normal);
        Debug.DrawRay(_player.transform.position, wallRunDirection, Color.black);

        stateParamsVelocity.x = wallRunDirection.x;
        stateParamsVelocity.z = wallRunDirection.z; 

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

    public IStateParams OnEnter(IStateParams stateParams)
    {
        return stateParams;
    }

    public IStateParams OnExit(IStateParams stateParams)
    {
        return stateParams;
    }
}