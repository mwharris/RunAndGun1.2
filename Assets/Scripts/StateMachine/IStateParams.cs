using UnityEngine;

public interface IStateParams
{
    Vector3 Velocity { get; set; }
    float GravityOverride { get; set; }
    RaycastHit WallRunHitInfo { get; set; }
    bool WallJumped { get; set; }
}