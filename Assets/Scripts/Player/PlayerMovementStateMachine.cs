using System;
using UnityEngine;

public class PlayerMovementStateMachine : MonoBehaviour
{
    private BaseStateMachine _stateMachine;
    private CharacterController _characterController;
    private IStateParams _stateParams;
    
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _horizontalVelocity = Vector3.zero;
    private float defaultGravity = -14f;
    private bool _preserveSprint = false;
    private bool _fixInitialNotGrounded = true;
    
    private bool _isWallRunning = false;
    
    public Type CurrentStateType => _stateMachine.CurrentState.GetType();
    public bool IsGrounded => _characterController.isGrounded;

    [SerializeField] private bool playerIsGrounded;

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
        WallRunning wallRunning = new WallRunning(player, defaultGravity);
        Crouching crouching = new Crouching(player);
        Sliding sliding = new Sliding(player);

        // Any -> Idle
        _stateMachine.AddAnyTransition(idle, () => idle.IsIdle() && !jumping.IsJumping() && (!crouching.IsCrouching && !crouching.Rising));
        // Any -> Jumping
        _stateMachine.AddAnyTransition(jumping, () => Jump(jumping));

        // Idle -> Walking
        _stateMachine.AddTransition(idle, walking, () => walking.IsWalking());
        // Walking -> Sprinting
        _stateMachine.AddTransition(walking, sprinting, () => PlayerInput.Instance.ShiftDown);
        // Sprinting -> Walking
        _stateMachine.AddTransition(sprinting, walking, () => !sprinting.IsStillSprinting());
        
        // Idle -> Crouching
        _stateMachine.AddTransition(idle, crouching, () => PlayerInput.Instance.CrouchDown);
        
        // Walking -> Crouching
        // Sprinting -> Sliding (Crouching)
        
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

    private bool Jump(Jumping jumping)
    {
        bool wallJumped = !_isWallRunning || _stateParams.WallJumped;
        return jumping.IsJumping() && !FixInitialNotGrounded() && wallJumped;
    }

    private void Update()
    {
        if (_characterController.isGrounded && _velocity.y < 0.1f)
        {
            _velocity.y = -2.5f;
        }

        playerIsGrounded = _characterController.isGrounded;
        
        // Wall-running raycast and hit info checks
        DoWallRunCheck(_stateParams, _velocity, _characterController.isGrounded);

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

        //DebugPrintVelocity();
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
        if (from is Sprinting && to is Jumping)
        {
            _preserveSprint = true;
        }
        else if (_preserveSprint && from is Jumping && !(to is Sprinting) && !(to is WallRunning))
        {
            _preserveSprint = false;
        }
        else if (from is WallRunning)
        {
            _isWallRunning = false;
        }
    }

    private void DoWallRunCheck(IStateParams stateParams, Vector3 velocity, bool isGrounded)
    {
        float rayDistance = 1f;
        
        if (!isGrounded)
        {
            var lastHitInfo = stateParams.WallRunHitInfo;
            // Check initialization of wall-running rules.
            // Right now this just mean checking if our velocity vector touches any walls.
            if (!_isWallRunning)
            {
                RaycastHit velocityHitInfo;
                Vector3 vDir = new Vector3(velocity.x, 0, velocity.z);
                Physics.Raycast(transform.position, vDir, out velocityHitInfo, rayDistance);
                Debug.DrawRay(transform.position, Vector3.ClampMagnitude(vDir, rayDistance), Color.yellow);
                if (velocityHitInfo.collider != null)
                {
                    _isWallRunning = true;
                    stateParams.WallRunHitInfo = velocityHitInfo;
                    return;
                }
            }
            // Check continuous wall-running rules.
            // Raycast along the last hit info's normal, in the reverse direction, to see if we're still on a wall.
            else if (lastHitInfo.collider != null)
            {
                RaycastHit wallNormalHitInfo;
                Vector3 rayDir = new Vector3(-lastHitInfo.normal.x, 0, -lastHitInfo.normal.z);
                Physics.Raycast(transform.position, rayDir, out wallNormalHitInfo, rayDistance);
                if (wallNormalHitInfo.collider != null)
                {
                    stateParams.WallRunHitInfo = wallNormalHitInfo;
                    return;
                }
            }
        }
        // If we reached this point we shouldn't be wall-running
        _isWallRunning = false;
        // This is here to make sure we're not creating new RaycastHits every frame
        if (stateParams.WallRunHitInfo.collider != null)
        {
            stateParams.WallRunHitInfo = new RaycastHit();
        }
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
                         /*+ ", Magnitude: " + _velocity.magnitude*/
        );
    }

    private void DebugWallRunRaycast()
    {
        Vector3 vDir = new Vector3(_velocity.x, 0, _velocity.z);
        Debug.DrawRay(transform.position, vDir, Color.yellow);
    }
}