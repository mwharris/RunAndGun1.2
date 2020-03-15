using UnityEngine;

public class PlayerMovementStateMachine : MonoBehaviour
{
    private BaseStateMachine _stateMachine;
    private CharacterController _characterController;

    private IStateParams _stateParams;
    
    private Vector3 _velocity = Vector3.zero;
    [SerializeField] private float gravity = -9.81f;

    private void Awake()
    {
        Player player = FindObjectOfType<Player>();
        _characterController = GetComponent<CharacterController>();
        _stateMachine = new BaseStateMachine();
        
        _stateParams = new StateParams();
        _stateParams.Velocity = _velocity;
        
        Walking walking = new Walking(player);
        Sprinting sprinting = new Sprinting(player);
        
        // Walking -> Sprinting transition
        _stateMachine.AddStateTransition(new StateTransition(
            walking,
            sprinting,
            () => PlayerInput.Instance.ShiftPressed
        ));
        
        // Sprinting -> Walking transition
        _stateMachine.AddStateTransition(new StateTransition(
            sprinting,
            walking,
            () => !PlayerInput.Instance.ShiftPressed
        ));
        
        _stateMachine.SetState(walking);
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

        // Handle our horizontal movement
        _characterController.Move(_velocity * Time.deltaTime);
        
        // Apply gravity (it's applied twice because t-squared
        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

        // PrintDebugVelocity();
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