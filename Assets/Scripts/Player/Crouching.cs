using System;
using System.Collections;
using UnityEngine;

public class Crouching : IState
{
    public bool IsCrouching { get; private set; } = false;
    
    private readonly Player _player;
    private CharacterController _characterController;
    private PlayerMovementStateMachine _movementStateMachine;
    private Transform _playerBody;
    private Transform _playerCamera;
    
    private float originalCharacterHeight;
    private float originalCameraHeight;

    private bool _firstframe = false;
    private bool _lowering = false;
    private bool _rising = false;
    public bool Rising => _rising;

    public Crouching(Player player)
    {
        _player = player;
        _playerBody = player.PlayerBody;
        _playerCamera = player.PlayerCamera.transform;
        _movementStateMachine = player.GetComponent<PlayerMovementStateMachine>();
        _characterController = player.GetComponent<CharacterController>();
        
        originalCharacterHeight = _characterController.height;
        originalCameraHeight = _playerCamera.transform.localPosition.y;
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        // Toggling between crouching and standing
        if (!_firstframe && PlayerInput.Instance.CrouchDown)
        {
            _lowering = !_lowering;
            _rising = !_rising;
        }
        else
        {
            _firstframe = false;
        }
        
        // Perform the actual crouch / stand
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
        var targetCcHeight = originalCharacterHeight / 2;
        var targetCameraHeight = originalCameraHeight / 2;
        
        var playerBodyScale = _playerBody.localScale;
        var ccHeight = _characterController.height;
        var ccCenter = _characterController.center;
        var cameraPosition = _playerCamera.transform.localPosition;

        playerBodyScale.y = Lower(playerBodyScale.y, targetBodyScale, Time.deltaTime * 8f);
        ccHeight = Lower(ccHeight, targetCcHeight, Time.deltaTime * 4f);
        ccCenter = Vector3.down * (2f - ccHeight) / 2.0f;
        cameraPosition.y = Lower(cameraPosition.y, targetCameraHeight, Time.deltaTime * 8f);
        
        _playerBody.localScale = playerBodyScale;
        _characterController.height = ccHeight;
        _characterController.center = ccCenter;
        _playerCamera.transform.localPosition = cameraPosition;
    }

    private void Stand()
    {
        IsCrouching = false;
        var targetBodyScale = 1f;
        var targetCcHeight = originalCharacterHeight;
        var targetCameraHeight = originalCameraHeight;
        
        var playerBodyScale = _playerBody.localScale;
        var ccHeight = _characterController.height;
        var ccCenter = _characterController.center;
        var cameraPosition = _playerCamera.transform.localPosition;

        playerBodyScale.y = Raise(playerBodyScale.y, targetBodyScale, Time.deltaTime * 16f);
        ccHeight = Raise(ccHeight, targetCcHeight, Time.deltaTime * 8f);
        ccCenter = Vector3.down * (originalCharacterHeight - ccHeight) / 2.0f;
        cameraPosition.y = Raise(cameraPosition.y, targetCameraHeight, Time.deltaTime * 8f);
        
        _playerBody.localScale = playerBodyScale;
        _characterController.height = ccHeight;
        _characterController.center = ccCenter;
        _playerCamera.transform.localPosition = cameraPosition;
    }
    
    private float Lower(float value, float target, float deltaTime)
    {
        if (value > target && Mathf.Abs(value - target) > 0.01f)
        {
            return Mathf.Lerp(value, target, deltaTime);
        }
        else
        {
            _lowering = false;
            return target;
        }
    }

    private float Raise(float value, float target, float deltaTime)
    {
        if (value < target && Mathf.Abs(value - target) > 0.01f)
        {
            return Mathf.Lerp(value, target, deltaTime);
        }
        else
        {
            _rising = false;
            return target;
        }
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