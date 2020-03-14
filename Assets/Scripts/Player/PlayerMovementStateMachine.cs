using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementStateMachine : MonoBehaviour
{
    private BaseStateMachine _stateMachine;
    private CharacterController _characterController;

    private Vector3 _velocity = Vector3.zero;
    private IStateParams _stateParams;

    private void Awake()
    {
        Player player = FindObjectOfType<Player>();
        _characterController = GetComponent<CharacterController>();
        _stateMachine = new BaseStateMachine();
        
        _stateParams = new StateParams();
        _stateParams.Velocity = _velocity;
        
        Walking walking = new Walking(player);
        _stateMachine.SetState(walking);
    }

    private void Update()
    {
        _stateParams.Velocity = _velocity;
        _stateParams = _stateMachine.Tick(_stateParams);
        _velocity = _stateParams.Velocity;

        _characterController.SimpleMove(_velocity);
    }
}