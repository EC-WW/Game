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

    Vec3 childPos = childTransform.GetPosition();
    Vec3 childRot = childTransform.GetRotation();

    tempOffsetX = childPos.x;
    tempOffsetY = childPos.y;
    tempOffsetZ = childPos.z;

    tempRotOffX = childRot.x;
    tempRotOffY = childRot.y;
    tempRotOffZ = childRot.z;
  }

  public override void Update()
  {
    Vec3 pos = transform.GetPosition();
    Vec3 rot = transform.GetRotation();

    //get the camera's forward direction
    // Convert rotation angles to radians
    float radX = rot.x;
    float radY = rot.y;
    float radZ = rot.z;

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
    targetPos = pos +
      (forward * -tempOffsetX) +
      (up * tempOffsetY) +
      (right * -tempOffsetZ);

    //calculate target rotation
    targetRot = new Vec3(
      rot.x + tempRotOffX,
      rot.y + tempRotOffY,
      rot.z + tempRotOffZ);

    childTransform.SetPosition(targetPos);
    childTransform.SetRotation(targetRot);
  }

  public override void Exit()
  {

  }
}
