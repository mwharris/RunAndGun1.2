using System;

public class Sliding : IState
{
    private readonly Player _player;
    
    public Sliding(Player player)
    {
        _player = player;
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        return stateParams;
    }

    public IStateParams OnEnter(IStateParams stateParams)
    {
        return stateParams;
    }

    public IStateParams OnExit(IStateParams stateParams)
    {
        return stateParams;
    }
}