using UnityEngine;

public class Walking : IState
{
    private CharacterController _characterController;
    
    public Walking(CharacterController characterController)
    {
        _characterController = characterController;
    }
    
    public void Tick(StateParams stateParams)
    {
        if (stateParams != null)
        {
            Vector3 velocity = stateParams.Velocity;
            velocity = new Vector3(PlayerInput.Instance.Horizontal, 0, PlayerInput.Instance.Vertical);
            _characterController.Move(velocity);
        }
    }

    public void OnEnter() { }

    public void OnExit() { }
}