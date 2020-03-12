using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementStateMachine : MonoBehaviour
{
    private BaseStateMachine _stateMachine;
    private StateParams _stateParams;

    private CharacterController _characterController;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        
        _stateMachine = new BaseStateMachine();
        _stateParams = new StateParams(_velocity);
        
        Walking walking = new Walking(_characterController);
        _stateMachine.SetState(walking);
    }

    void Update()
    {
        _stateParams.Velocity = _velocity;
        
        // TODO: this needs to both pass in velocity and get back velocity...
        _stateMachine.Tick(_stateParams);
        
        _velocity = _stateParams.Velocity;
        
        // TODO: handle actually moving here...
        //Vector3 movement = transform.rotation * movementInput;
        //_characterController.SimpleMove(movement);
    }
}