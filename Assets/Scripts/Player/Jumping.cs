using System;
using UnityEngine;

public class Jumping : IState
{
    private readonly Player _player;
    private readonly CharacterController _characterController;
    
    private float jumpSpeed = 5f;
    private bool _doJump = false;
    private bool _doubleJumpAvailable = true;
    private float _startingMagnitude = 0f;
    
    private bool JumpDown => PlayerInput.Instance.SpaceDown;
    private bool JumpHeld => PlayerInput.Instance.SpaceHeld;
    
    public Jumping(Player player)
    {
        _player = player;
        _characterController = player.GetComponent<CharacterController>();
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        var stateParamsVelocity = stateParams.Velocity;

        stateParamsVelocity = HandleJumping(stateParamsVelocity);
        stateParamsVelocity = HandleMovement(stateParamsVelocity);

        stateParams.Velocity = stateParamsVelocity;
        return stateParams;
    }

    private Vector3 HandleMovement(Vector3 velocity)
    {
        // Get the raw axis input
        float forwardSpeed = PlayerInput.Instance.VerticalRaw;
        float sideSpeed = PlayerInput.Instance.HorizontalRaw;
        
        // Create of a vector of our input direction relative to the player's forward direction
        var inputVelocity = (_player.transform.forward * forwardSpeed) + (_player.transform.right * sideSpeed);
        Debug.DrawRay(_player.transform.position, inputVelocity * 3, Color.blue);
        
        // Create a vector from our current velocity ignoring the y-axis
        var tempVelocity = new Vector3(velocity.x, 0f, velocity.z);
        
        // Determine the angle (in degrees) between this direction and our velocity direction
        float angle = Vector3.Angle(tempVelocity, inputVelocity);
        if (angle > 10)
        {
            float startingMagnitude = velocity.magnitude;
            velocity += inputVelocity * 0.1f;
            velocity = Vector3.ClampMagnitude(velocity, startingMagnitude);
        }
        
        Vector3 debugV = new Vector3(velocity.x, 0f, velocity.z);
        Debug.Log("Magnitude: " + debugV.magnitude);
        
        return velocity;
    }

    private Vector3 HandleJumping(Vector3 velocity)
    {
        // Jump when we first enter the jump state
        if (_doJump)
        {
            velocity.y = jumpSpeed;
            _doJump = false;
        }
        // Jump when we hit the ground if we're holding the jump button
        else if (_characterController.isGrounded && JumpHeld)
        {
            velocity.y = jumpSpeed;
            _doubleJumpAvailable = true;
        }
        // Jump if we have a double jump and we hit Jump button
        else if (_doubleJumpAvailable && JumpDown)
        {
            velocity.y = jumpSpeed;
            _doubleJumpAvailable = false;
        }
        return velocity; 
    }

    public IStateParams OnEnter(IStateParams stateParams)
    {
        if (PlayerInput.Instance.SpaceDown)
        {
            _doJump = true;
        }
        _startingMagnitude = stateParams.Velocity.magnitude;
        return stateParams;
    }

    public IStateParams OnExit(IStateParams stateParams)
    {
        _doubleJumpAvailable = true;
        _startingMagnitude = 0f;
        return stateParams;
    }

    public bool IsJumping()
    {
        // If we hit or are holding the Jump button
        if (JumpDown || JumpHeld)
        {
            return true;
        }
        // If we are still airborne
        else if (!_characterController.isGrounded)
        {
            return true;
        }
        return false;
    }
}