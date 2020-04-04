using System;
using System.Collections;
using UnityEngine;

public class Crouching : IState
{
    private readonly Player _player;
    private CharacterController _characterController;
    private PlayerMovementStateMachine _movementStateMachine;
    private Transform _playerBody;

    public bool IsCrouching { get; private set; } = false;

    private bool _firstframe = false;
    private bool _lowering = false;
    private bool _rising = false;
    public bool Rising => _rising;

    public Crouching(Player player)
    {
        _player = player;
        _playerBody = player.PlayerBody;
        _characterController = player.GetComponent<CharacterController>();
        _movementStateMachine = player.GetComponent<PlayerMovementStateMachine>();
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        if (!_firstframe && PlayerInput.Instance.CrouchDown)
        {
            _lowering = !_lowering;
            _rising = !_rising;
        }
        else
        {
            _firstframe = false;
        }
        
        if (_lowering)
        {
            Crouch();
        }
        else if (_rising)
        {
            Stand();   
        }
        
        // TODO: Handle movement

        return stateParams;
    }

    private void Crouch()
    {
        IsCrouching = true;
        var targetBodyScale = 0.5f;
        var playerBodyScale = _playerBody.localScale;
        if (playerBodyScale.y > targetBodyScale && Mathf.Abs(playerBodyScale.y - targetBodyScale) > 0.01f)
        {
            playerBodyScale.y = Mathf.Lerp(playerBodyScale.y, targetBodyScale, Time.deltaTime * 8f);
        }
        else
        {
            playerBodyScale.y = targetBodyScale;
            _lowering = false;
        }
        _playerBody.localScale = playerBodyScale;
    }
    
    private void Stand()
    {
        IsCrouching = false;
        var targetBodyScale = 1f;
        var playerBodyScale = _playerBody.localScale;
        if (playerBodyScale.y < targetBodyScale && Mathf.Abs(playerBodyScale.y - targetBodyScale) > 0.01f)
        {
            playerBodyScale.y = Mathf.Lerp(playerBodyScale.y, targetBodyScale, Time.deltaTime * 8f);
        }
        else
        {
            playerBodyScale.y = targetBodyScale;
            _rising = false;
        }
        _playerBody.localScale = playerBodyScale;
    }

    
    public IStateParams OnEnter(IStateParams stateParams)
    {
        _lowering = true;
        _firstframe = true;
        return stateParams;
    }

    public IStateParams OnExit(IStateParams stateParams)
    {
        return stateParams;
    }

    
    
}