using UnityEngine;

public class Walking : IState
{
    private readonly Player _player;
    
    // 3.4 m/s
    private float _walkingSpeed = 3.4f;

    public Walking(Player player)
    {
        _player = player;
    }
    
    public IStateParams Tick(IStateParams stateParams)
    {
        Vector3 velocity;

        // Gather our vertical and horizontal input
        float forwardSpeed = PlayerInput.Instance.Vertical;
        float sideSpeed = PlayerInput.Instance.Horizontal;

        // Apply these values to our player
        velocity = (_player.transform.forward * forwardSpeed) + (_player.transform.right * sideSpeed);
        velocity *= _walkingSpeed;
        
        // Make sure we're never moving faster than our walking speed
        velocity = Vector3.ClampMagnitude(velocity, _walkingSpeed);
        
        stateParams.Velocity = velocity;
        return stateParams;
    }

    public void OnEnter()
    {
        Debug.Log("Entering Walking...");
    }

    public void OnExit()
    {
        Debug.Log("Exiting Walking...");
    }
}