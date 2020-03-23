using System;
using UnityEngine;

public class PlayerMovementStateMachine : MonoBehaviour
{
    private BaseStateMachine _stateMachine;
    private CharacterController _characterController;
    private IStateParams _stateParams;
    
    [SerializeField] private Transform downcastPoint;
    
    private float defaultGravity = -14f;
    
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _horizontalVelocity = Vector3.zero;
    
    private bool _preserveSprint = false;
    private bool _isWallRunning = false;
    private bool _fixInitialNotGrounded = true;
    
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
        _stateParams.GravityOverride = defaultGravity;

        // Create our states
        Idle idle = new Idle(player);
        Walking walking = new Walking(player);
        Sprinting sprinting = new Sprinting(player);
        Jumping jumping = new Jumping(player);
        WallRunning wallRunning = new WallRunning(defaultGravity);

        // Any -> Idle
        _stateMachine.AddAnyTransition(idle, () => idle.IsIdle() && !jumping.IsJumping());
        // Any -> Jumping
        _stateMachine.AddAnyTransition(jumping, () => jumping.IsJumping() && !_isWallRunning && !FixInitialNotGrounded());

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
        
        // Jumping -> Wall Running
        _stateMachine.AddTransition(jumping, wallRunning, () => _isWallRunning);
        // Wall Running -> Sprinting
        _stateMachine.AddTransition(wallRunning, jumping, () => !_isWallRunning && !jumping.IsJumping() && _preserveSprint);
        // Wall Running -> Walking
        _stateMachine.AddTransition(wallRunning, jumping, () => !_isWallRunning && !jumping.IsJumping() && walking.IsWalking());

        // Default to Idle
        _stateParams = _stateMachine.SetState(idle, _stateParams);
    }

    private void Update()
    {
        if (_characterController.isGrounded && _velocity.y < 0.1f)
        {
            _velocity.y = -2.5f;
        }
        
        // Wall-running raycasts
        _isWallRunning = DoWallRunCheck(_velocity, _characterController.isGrounded);

        // Tick our current state to handle our movement
        _stateParams.Velocity = _velocity;
        _stateParams = _stateMachine.Tick(_stateParams);
        _velocity = _stateParams.Velocity;
        
        // Update our horizontal velocity variable
        _horizontalVelocity.x = _velocity.x;
        _horizontalVelocity.z = _velocity.z;

        // Handle our horizontal movement
        _characterController.Move(_velocity * Time.deltaTime);
        
        // Apply gravity (call move twice because t-squared)
        HandleGravity();

        DebugWallRunRacyast();
    }

    private void HandleGravity()
    {
        if (_stateMachine.CurrentState is WallRunning)
        {
            _velocity.y += _stateParams.GravityOverride * Time.deltaTime;
        }
        else if (_velocity.y > 0)
        {
            _velocity.y += defaultGravity * Time.deltaTime;
        }
        else
        {
            _velocity.y += (defaultGravity * 1.25f) * Time.deltaTime;
        }
        _characterController.Move(_velocity * Time.deltaTime);
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

    private bool DoWallRunCheck(Vector3 velocity, bool isGrounded)
    {
        if (!isGrounded)
        {
            RaycastHit vHit;
            Vector3 vDir = new Vector3(velocity.x, 0, velocity.z);
            Physics.Raycast(transform.position, vDir, out vHit, 1);
            if (vHit.collider != null)
            {
                return true;
            }
        }
        return false;
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
    
    private void DebugPrintVelocity()
    {
        Vector3 horizontalVelocity = new Vector3(_velocity.x, 0f, _velocity.z);
        Debug.Log(
            "Velocity: " + _velocity 
                         + ", Horiz. Magnitude: " + horizontalVelocity.magnitude
                         + ", Magnitude: " + _velocity.magnitude
        );
    }

    private void DebugWallRunRacyast()
    {
        Vector3 vDir = new Vector3(_velocity.x, 0, _velocity.z);
        Debug.DrawRay(transform.position, vDir, Color.yellow);
    }
}