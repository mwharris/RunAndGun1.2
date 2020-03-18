using System;
using UnityEngine;

public class PlayerMovementStateMachine : MonoBehaviour
{
    private BaseStateMachine _stateMachine;
    private CharacterController _characterController;
    private IStateParams _stateParams;
    
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _horizontalVelocity = Vector3.zero;
    [SerializeField] private float gravity = -9.81f;

    public Type CurrentStateType => _stateMachine.CurrentState.GetType();
    
    private void Awake()
    {
        Player player = FindObjectOfType<Player>();
        _characterController = GetComponent<CharacterController>();
        _stateMachine = new BaseStateMachine();
        
        _stateParams = new StateParams();
        _stateParams.Velocity = _velocity;
        
        Idle idle = new Idle(player);
        Walking walking = new Walking(player);
        Sprinting sprinting = new Sprinting(player);

        // Any -> Idle
        _stateMachine.AddAnyTransition(idle, () => idle.CheckIdle());
        // Idle -> Walking transition
        _stateMachine.AddTransition(idle, walking, () => !idle.CheckIdle());
        // Walking -> Sprinting transition
        _stateMachine.AddTransition(walking, sprinting, () => PlayerInput.Instance.ShiftDown);
        // Sprinting -> Walking transition
        _stateMachine.AddTransition(sprinting, walking, () => !sprinting.IsStillSprinting());

        _stateMachine.SetState(idle);
    }

    private void Update()
    {
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        
        // Tick our current state to handle our movement
        _stateParams.Velocity = _velocity;
        _stateParams = _stateMachine.Tick(_stateParams);
        _velocity = _stateParams.Velocity;
        
        // Update our horizontal velocity variable
        _horizontalVelocity.x = _velocity.x;
        _horizontalVelocity.z = _velocity.z;

        // Handle our horizontal movement
        _characterController.Move(_velocity * Time.deltaTime);
        
        // Apply gravity (it's applied twice because t-squared
        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

        // PrintDebugVelocity();
    }
    
    private bool NoMovement()
    {
        var noHorizontal = !PlayerInput.Instance.HorizontalHeld;
        var noVertical = !PlayerInput.Instance.VerticalHeld;
        var noVelocity = _horizontalVelocity.magnitude < 0.1f;
        return noHorizontal && noVertical && noVelocity;
    }

    private void PrintDebugVelocity()
    {
        Vector3 horizontalVelocity = new Vector3(_velocity.x, 0f, _velocity.z);
        Debug.Log(
    "Velocity: " + _velocity 
                 + ", Horiz. Magnitude: " + horizontalVelocity.magnitude
                 + ", Magnitude: " + _velocity.magnitude
        );
    }
}