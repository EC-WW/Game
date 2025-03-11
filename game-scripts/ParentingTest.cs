/**
/ file:   ParentingTest.cs
/ author: taylor.cadwallader
/ date:   March 3, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  Script that handles fake parenting for entities not including the camera.
**/
using System;
using NITELITE;

public class ParentingTest : NL_Script
{
  public Entity Child;

  //private Vec3 DepthOffset = new Vec3(1f, -0.3f, 0.6f);
  public float tempOffsetX = 1f;
  public float tempOffsetY = -0.3f;
  public float tempOffsetZ = 0.6f;

  public float tempRotOffX = 180f;
  public float tempRotOffY = 0f;
  public float tempRotOffZ = 180f;

  private Vec3 targetPos;
  private Vec3 targetRot;

  private Transform transform;
  private Transform childTransform;

  public override void Init()
  {
    transform = self.GetComponent<Transform>();
    childTransform = Child.GetComponent<Transform>();

    tempOffsetX = childTransform.position.x;
    tempOffsetY = childTransform.position.y;
    tempOffsetZ = childTransform.position.z;

    tempRotOffX = childTransform.rotation.x;
    tempRotOffY = childTransform.rotation.y;
    tempRotOffZ = childTransform.rotation.z;
  }

  public override void Update()
  {
    //get the camera's forward direction
    // Convert rotation angles to radians
    float radX = transform.rotation.x;
    float radY = transform.rotation.y;
    float radZ = transform.rotation.z;

    // Calculate the full rotation matrix using Euler angles
    Vec3 forward = new Vec3(
        MathF.Cos(radY) * MathF.Cos(radX),
        MathF.Sin(radX),
        MathF.Sin(radY) * MathF.Cos(radX)
    );

    Vec3 right = new Vec3(
        MathF.Cos(radY) * MathF.Sin(radZ) + MathF.Sin(radY) * MathF.Sin(radX) * MathF.Cos(radZ),
        MathF.Cos(radX) * MathF.Cos(radZ),
        MathF.Sin(radY) * MathF.Sin(radZ) - MathF.Cos(radY) * MathF.Sin(radX) * MathF.Cos(radZ)
    );

    Vec3 up = new Vec3(
        MathF.Cos(radY) * MathF.Cos(radZ) - MathF.Sin(radY) * MathF.Sin(radX) * MathF.Sin(radZ),
        MathF.Cos(radX) * MathF.Sin(radZ),
        MathF.Sin(radY) * MathF.Cos(radZ) + MathF.Cos(radY) * MathF.Sin(radX) * MathF.Sin(radZ)
    );
    // Normalize vectors
    forward.Normalize();
    right.Normalize();
    up.Normalize();

    //calculate target position w/ depth offset
    targetPos = transform.position +
      (forward * -tempOffsetX) +
      (up * tempOffsetY) +
      (right * -tempOffsetZ);

    //calculate target rotation
    targetRot = new Vec3(
      transform.rotation.x + tempRotOffX,
      transform.rotation.y + tempRotOffY,
      transform.rotation.z + tempRotOffZ);

    childTransform.position = targetPos;
    childTransform.rotation = targetRot;
  }

  public override void Exit()
  {

  }

  private Vec3 CrossProduct(Vec3 a, Vec3 b)
  {
    return new Vec3(
        (a.y * b.z) - (a.z * b.y),
        (a.z * b.x) - (a.x * b.z),
        (a.x * b.y) - (a.y * b.x)
    );
  }
}
