using NITELITE;
using System;

public class LobbyHeightmap : NL_Script
{
  public Entity Player;          // Assign in inspector
  public float Xf = 10f;         // Distance before scaling starts
  public float ScaleRate = 0.1f; // How quickly it scales outward

  private Transform _transform;
  private const float MinScaleXZ = 20f;

  public override void Init()
  {
    _transform = self.GetComponent<Transform>();
  }

  public override void Update()
  {
    Vec3 playerPos = Player.GetComponent<Transform>().GetPosition();

    // Distance from origin on XZ plane
    Vec2 flatPos = new Vec2(playerPos.x, playerPos.z);
    float distanceFromOrigin = flatPos.Magnitude();

    float excessDistance = Math.Max(0f, distanceFromOrigin - Xf);

    // Scale factor based on distance
    float scaleXZ = MinScaleXZ + (excessDistance * ScaleRate);

    // Apply scale, keep original Y
    Vec3 currentScale = _transform.GetScale();
    _transform.SetScale(new Vec3(scaleXZ, currentScale.y, scaleXZ));
  }

  public override void Exit()
  {
    
  }
}