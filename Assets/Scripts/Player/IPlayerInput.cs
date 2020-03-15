public interface IPlayerInput
{
    float Vertical { get; }
    float Horizontal { get; }
    
    float MouseX { get; }
    float MouseY { get; }
    
    bool ShiftPressed { get; }
    
    void Tick();
}
