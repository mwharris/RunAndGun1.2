using System;
using UnityEngine;

public class Jumping : IState
{
    private readonly Player _player;
    private readonly CharacterController _characterController;

    private float jumpSpeed = 10f;

    private bool doJump = false;
    private bool doubleJumpAvailable = true;
    
    public Jumping(Player player)
    {
        _player = player;
        _characterController = player.GetComponent<CharacterController>();
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        var stateParamsVelocity = stateParams.Velocity;
        
        // Jump when we first enter the jump state
        if (doJump)
        {
            stateParamsVelocity.y = jumpSpeed;
            doJump = false;
        }
        /*
        // Jump if we have a double jump and we hit Jump button
        else if (doubleJumpAvailable && PlayerInput.Instance.SpaceDown)
        {
            Jump(stateParams.Velocity);
            doubleJumpAvailable = false;
        }
        */
        
        // Update our stateParams velocity
        stateParams.Velocity = stateParamsVelocity;

        return stateParams;
    }
    
    private void Jump(Vector3 velocity)
    {
        velocity.y = jumpSpeed;
    }

    public bool IsJumping()
    {
        // If we just hit the Jump button
        if (PlayerInput.Instance.SpaceDown)
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


    public void OnEnter()
    {
        if (PlayerInput.Instance.SpaceDown)
        {
            doJump = true;
        }
    }

    public void OnExit()
    {
    }
}