using System;

public class Crouching : IState
{
    private readonly Player _player;

    public Crouching(Player player)
    {
        _player = player;
    }

    public IStateParams Tick(IStateParams stateParams)
    {
        throw new NotImplementedException();
    }

    public IStateParams OnEnter(IStateParams stateParams)
    {
        throw new NotImplementedException();
    }

    public IStateParams OnExit(IStateParams stateParams)
    {
        throw new NotImplementedException();
    }
}