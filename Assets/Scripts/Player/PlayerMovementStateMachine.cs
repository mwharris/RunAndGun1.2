using System;
using UnityEngine;

public class PlayerMovementStateMachine : MonoBehaviour
{
    private BaseStateMachine _stateMachine;
    private CharacterController _characterController;
    private IStateParams _stateParams;
    
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform downcastPoint;
    
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _horizontalVelocity = Vector3.zero;
    
    private bool _preserveSprint = false;
    private bool _fixInitialNotGrounded = true;
    private bool _bunnyHopAvailable = false;

    public Type CurrentStateType => _stateMachine.CurrentState.GetType();
    public bool IsGrounded => _characterController.isGrounded;

    private void Awake()
    {
        Player player = FindObjectOfType<Player>();
        _characterController = GetComponent<CharacterController>();
        _stateMachine = new BaseStateMachine();
        
        // Hook into the BaseStateMachine OnStateChanged event
        _stateMachine.OnStateChanged += HandleStateChanged;
        
        // Prepare our StateParams for passing to all of our states
        _stateParams = new StateParams();
        _stateParams.Velocity = _velocity;

        // Create our states
        Idle idle = new Idle(player);
        Walking walking = new Walking(player);
        Sprinting sprinting = new Sprinting(player);
        Jumping jumping = new Jumping(player);

        // Any -> Idle
        _stateMachine.AddAnyTransition(idle, () => idle.IsIdle() && !jumping.IsJumping());
        // Any -> Jumping
        _stateMachine.AddAnyTransition(jumping, () => jumping.IsJumping() && !FixInitialNotGrounded());

        // Idle -> Walking
        _stateMachine.AddTransition(idle, walking, () => walking.IsWalking());
        // Walking -> Sprinting
        _stateMachine.AddTransition(walking, sprinting, () => PlayerInput.Instance.ShiftDown);
        // Sprinting -> Walking
        _stateMachine.AddTransition(sprinting, walking, () => !sprinting.IsStillSprinting());
        
        // Jumping -> Sprinting
        _stateMachine.AddTransition(jumping, sprinting, () => !jumping.IsJumping() && _preserveSprint);
        // Jumping -> Walking
        _stateMachine.AddTransition(jumping, walking, () => !jumping.IsJumping() && walking.IsWalking());

        // Default to Idle
        _stateMachine.SetState(idle);
    }

    private void HandleStateChanged(IState from, IState to)
    {
        // Preserve Sprinting through our Jump
        if (from is Sprinting && to is Jumping)
        {
            _preserveSprint = true;
        }
        else if (_preserveSprint && from is Jumping && !(to is Sprinting))
        {
            _preserveSprint = false;
        }
    }

    private void Update()
    {
        if (_characterController.isGrounded && _velocity.y < 0.1f)
        {
            _velocity.y = -2.5f;
        }

        _stateParams.Velocity = _velocity;
        
        // Tick our current state to handle our movement
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
    }
    
    private bool FixInitialNotGrounded()
    {
        if (_fixInitialNotGrounded)
        {
            _fixInitialNotGrounded = false;
            return true;
        }
        return false;
    }
}