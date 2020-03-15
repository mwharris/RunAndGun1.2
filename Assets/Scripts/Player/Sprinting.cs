using UnityEngine;

public class Sprinting : IState
{
    private readonly Player _player;
    
    public Sprinting(Player player)
    {
        _player = player;
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        Debug.Log("Sprinting...");
        return stateParams;
    }

    public void OnEnter()
    {
        Debug.Log("Entering Sprinting...");
    }

    public void OnExit()
    {
        Debug.Log("Exiting Sprinting...");
    }
}