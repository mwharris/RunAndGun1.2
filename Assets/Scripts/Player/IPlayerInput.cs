public interface IPlayerInput
{
    float Vertical { get; }
    bool VerticalHeld { get; }
    float Horizontal { get; }
    bool HorizontalHeld { get; }
    
    float MouseX { get; }
    float MouseY { get; }
    
    bool ShiftDown { get; }
    bool ShiftHeld { get; }
    
    bool SpaceDown { get; }
    
    void Tick();
}
