using System;
using UnityEngine;

public class Jumping : IState
{
    private readonly CharacterController _characterController;

    private float jumpSpeed = 5f;
    private bool _doJump = false;
    private bool _doubleJumpAvailable = true;
    private bool JumpDown => PlayerInput.Instance.SpaceDown;
    private bool JumpHeld => PlayerInput.Instance.SpaceHeld;
    
    public Jumping(Player player)
    {
        _characterController = player.GetComponent<CharacterController>();
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        var stateParamsVelocity = stateParams.Velocity;

        // Jump when we first enter the jump state
        if (_doJump)
        {
            //Debug.Log("Enter Jump");
            stateParamsVelocity.y = jumpSpeed;
            _doJump = false;
        }
        else if (_characterController.isGrounded && JumpHeld)
        {
            //Debug.Log("Bunny Hop");
            stateParamsVelocity.y = jumpSpeed;
            _doubleJumpAvailable = true;
        }
        // Jump if we have a double jump and we hit Jump button
        else if (_doubleJumpAvailable && JumpDown)
        {
            //Debug.Log("Double Jump");
            stateParamsVelocity.y = jumpSpeed;
            _doubleJumpAvailable = false;
        }

        // Update our stateParams velocity
        stateParams.Velocity = stateParamsVelocity;
        
        return stateParams;
    }

    public void OnEnter()
    {
        if (PlayerInput.Instance.SpaceDown)
        {
            _doJump = true;
        }
    }

    public void OnExit()
    {
        _doubleJumpAvailable = true;
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