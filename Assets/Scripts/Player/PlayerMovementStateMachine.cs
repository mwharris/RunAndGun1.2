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
        Sprinting sprinting = new Sprinting(player);
        
        _stateMachine.SetState(walking);
    }

    private void Update()
    {
        _stateParams.Velocity = _velocity;
        _stateParams = _stateMachine.Tick(_stateParams);
        _velocity = _stateParams.Velocity;

        /*
         // TODO: not sure if we need this, leaving it here for now...
        //Linear drag along the X and Z while grounded
        if(_characterController.isGrounded)
        {
            _velocity.x *= 0.9f;
            _velocity.z *= 0.9f;
        }
        */
        
        Debug.Log("Velocity: " + _velocity + ", Magnitude: " + _velocity.magnitude);

        _characterController.Move(_velocity * Time.deltaTime);
    }
}