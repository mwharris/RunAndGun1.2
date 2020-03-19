using UnityEngine;

public class Walking : IState
{
    private readonly Player _player;
    private readonly CharacterController _characterController;
    
    // 3.4 m/s
    private float _walkingSpeed = 3.4f;

    public Walking(Player player)
    {
        _player = player;
        _characterController = player.GetComponent<CharacterController>();
    }
    
    public IStateParams Tick(IStateParams stateParams)
    {
        var stateParamsVelocity = stateParams.Velocity;

        // Gather our vertical and horizontal input
        float forwardSpeed = PlayerInput.Instance.Vertical;
        float sideSpeed = PlayerInput.Instance.Horizontal;

        // Apply these values to our player
        var tempVelocity = (_player.transform.forward * forwardSpeed) + (_player.transform.right * sideSpeed);
        tempVelocity *= _walkingSpeed;
        
        // Make sure we're never moving faster than our walking speed
        tempVelocity = Vector3.ClampMagnitude(tempVelocity, _walkingSpeed);
        
        // Update our stateParams velocity
        stateParamsVelocity.x = tempVelocity.x;
        stateParamsVelocity.z = tempVelocity.z;
        stateParams.Velocity = stateParamsVelocity;
        
        return stateParams;
    }

    public bool IsWalking()
    {
        var inputHeld = PlayerInput.Instance.HorizontalHeld || PlayerInput.Instance.VerticalHeld;
        return _characterController.isGrounded && inputHeld;
    }

    public void OnEnter()
    {
        // Debug.Log("Entering Walking...");
    }

    public void OnExit()
    {
        // Debug.Log("Exiting Walking...");
    }
}