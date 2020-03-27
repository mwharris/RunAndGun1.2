using UnityEngine;

public class WallRunning : IState
{
    private readonly Player _player;
    private readonly float _gravity;
    
    private readonly float _wallRunSpeed = 6.8f;
    private readonly float _wallRunSlowSpeed = 1f;

    public WallRunning(Player player, float defaultGravity)
    {
        _player = player;
        _gravity = defaultGravity;
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        var stateParamsVelocity = stateParams.Velocity;
        var wallRunHitInfo = stateParams.WallRunHitInfo;
        var horizontalVelocity = new Vector3(stateParamsVelocity.x, 0, stateParamsVelocity.z);

        var forwardSpeed = PlayerInput.Instance.Vertical;
        var sideSpeed = PlayerInput.Instance.Horizontal;
        var forwardHeld = PlayerInput.Instance.VerticalRaw;
        var sideHeld = PlayerInput.Instance.HorizontalRaw;

        // Find the direction parallel to the wall using the wallRunHitInfo.normal
        var wallRunMoveAxis = Vector3.Cross(Vector3.up, wallRunHitInfo.normal);

        // Use the player velocity to determine which way they should be headed along this axis
        float dot = Vector3.Dot(wallRunMoveAxis, horizontalVelocity);
        float magnitudeMult = wallRunMoveAxis.magnitude * horizontalVelocity.magnitude;
        float angle = Mathf.Acos(dot / magnitudeMult) * Mathf.Rad2Deg;
        if (angle > 90)
        {
            wallRunMoveAxis = -wallRunMoveAxis;
        }
        
        Debug.Log("Angle : " + angle);
        Debug.DrawRay(wallRunHitInfo.point, wallRunMoveAxis * 10, Color.green);
        
        // TODO: maybe just pick one if there's no distinction
        
        
        /*
        // Project our velocity parallel to the wall we're running along
        var wallRunDirection = Vector3.ProjectOnPlane(horizontalVelocity, wallRunHitInfo.normal);
        Debug.DrawRay(_player.transform.position, wallRunDirection, Color.black);

        wallRunDirection = wallRunDirection * forwardSpeed;
        wallRunDirection *= _wallRunSpeed;
        wallRunDirection = Vector3.ClampMagnitude(wallRunDirection, _wallRunSpeed);

        stateParamsVelocity.x = wallRunDirection.x;
        stateParamsVelocity.z = wallRunDirection.z;
        */
   
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