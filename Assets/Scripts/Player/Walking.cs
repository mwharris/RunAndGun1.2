using UnityEngine;

public class Walking : IState
{
    private readonly Player _player;

    public Walking(Player player)
    {
        _player = player;
    }
    
    public IStateParams Tick(IStateParams stateParams)
    {
        Vector3 velocity = stateParams.Velocity;
        velocity = new Vector3(PlayerInput.Instance.Horizontal, 0, PlayerInput.Instance.Vertical);
        velocity = _player.transform.rotation * velocity;

        stateParams.Velocity = velocity;
        return stateParams;
    }

    public void OnEnter()
    {
        Debug.Log("Entering Walking...");
    }

    public void OnExit()
    {
        Debug.Log("Exiting Walking...");
    }
}