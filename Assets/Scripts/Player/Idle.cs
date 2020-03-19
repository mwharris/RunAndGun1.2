using System;
using UnityEngine;

public class Idle : IState
{
    private readonly Player _player;
    private readonly CharacterController _characterController;
    
    public Idle(Player player)
    {
        _player = player;
        _characterController = player.GetComponent<CharacterController>();
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

    public bool IsIdle()
    {
        var noHorizontal = !PlayerInput.Instance.HorizontalHeld;
        var noVertical = !PlayerInput.Instance.VerticalHeld;
        return noHorizontal && noVertical && _characterController.isGrounded;
    }
    
    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

}