using UnityEngine;

public class Sprinting : IState
{
    private readonly Player _player;
    
    // 5.1 m/s
    private float _sprintingSpeed = 5.1f;
    
    public Sprinting(Player player)
    {
        _player = player;
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        var stateParamsVelocity = stateParams.Velocity;

        // Gather our vertical and horizontal input
        float forwardSpeed = PlayerInput.Instance.Vertical;
        float sideSpeed = PlayerInput.Instance.Horizontal;

        // Apply these values to our player
        var tempVelocity = (_player.transform.forward * forwardSpeed) + (_player.transform.right * sideSpeed);
        tempVelocity *= _sprintingSpeed;
            
        // Make sure we're never moving faster than our walking speed
        tempVelocity = Vector3.ClampMagnitude(tempVelocity, _sprintingSpeed);
            
        // Update our stateParams velocity
        stateParamsVelocity.x = tempVelocity.x;
        stateParamsVelocity.z = tempVelocity.z;
        stateParams.Velocity = stateParamsVelocity;
            
        return stateParams;
    }

    public void OnEnter()
    {
        // Debug.Log("Entering Sprinting...");
    }

    public void OnExit()
    {
        // Debug.Log("Exiting Sprinting...");
    }
}