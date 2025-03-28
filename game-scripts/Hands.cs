﻿/**
/ file:   Hands.cs
/ author: jared.callupelopez
/ date:   February 27, 2024
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  Script that handles player hands.
**/
using System;
using NITELITE;

public class Hands : NL_Script
{
  public Entity Camera;

  //private Vec3 DepthOffset = new Vec3(1f, -0.3f, 0.6f);
  public float tempOffsetX = 1f;
  public float tempOffsetY = -0.3f;
  public float tempOffsetZ = 0.6f;

  public float tempRotOffX = 180f;
  public float tempRotOffY = 0f;
  public float tempRotOffZ = 180f;

  public float MoveLerpSpeed = 1f;
  public float RotateLerpSpeed = 1f;

  private Vec3 targetPos;
  private Vec3 targetRot;

  private Transform transform;
  private Transform camTrans;

  private CameraComponent camComp;

  public override void Init()
  {
    transform = self.GetComponent<Transform>();
    camTrans = Camera.GetComponent<Transform>();
    camComp = Camera.GetComponent<CameraComponent>();
  }

  public override void Update()
  {
    FakeParenting(); // :3
    UpdateChildTransform();
  }

  private void FakeParenting()
  {
    //get the camera's forward direction
    Vec3 camForward = camComp.facingDirection;
    //calculate the right direction of the camera
    Vec3 camRight = new Vec3(camForward.z, 0, -camForward.x);
    camRight.Normalize();
    //calculate the up direction of the camera
    Vec3 camUp = CrossProduct(camRight, camForward);
    camUp.Normalize();

    Vec3 camPos = camTrans.GetPosition();
    Vec3 camRot = camTrans.GetRotation();

    //calculate target position w/ depth offset
    targetPos = camPos +
      (camForward * tempOffsetX) +
      (camUp * -tempOffsetY) +
      (camRight * tempOffsetZ);

    //calculate target rotation
    //flipping camera's x and y rotations accordingly to the entity's x and y
    //idk why, but the camera x and y values are flipped for some silly reason
    //x needs to be flipped again with the -1 so it correctly rotates to face outward
    targetRot = new Vec3(
      (DegreesToRadians(camComp.rotation.y) - DegreesToRadians(camRot.y)) * -1 + DegreesToRadians(tempRotOffX),
      DegreesToRadians(camComp.rotation.x) - DegreesToRadians(camRot.x) + DegreesToRadians(tempRotOffY),
      DegreesToRadians(tempRotOffZ));
  }

  private void UpdateChildTransform()
  {
    transform.SetPosition(targetPos);
    transform.SetPosition(targetRot);

    //lerp the positions and rotations of the hands
    //transform.position = LerpVec3(transform.position, targetPos, dt * MoveLerpSpeed);
    //transform.rotation = LerpVec3(transform.rotation, targetRot, dt * RotateLerpSpeed);
  }

  public override void Exit()
  {

  }

  private float DegreesToRadians(float degrees)
  {
    return degrees * ((float)Math.PI) / 180f;
  }

  private Vec3 CrossProduct(Vec3 a, Vec3 b)
  {
    return new Vec3(
        (a.y * b.z) - (a.z * b.y),
        (a.z * b.x) - (a.x * b.z),
        (a.x * b.y) - (a.y * b.x)
    );
  }

  public Vec3 LerpVec3(Vec3 startPos, Vec3 endPos, float t)
  {
    float x = Lerp(startPos.x, endPos.x, t);
    float y = Lerp(startPos.y, endPos.y, t);
    float z = Lerp(startPos.z, endPos.z, t);
    return new Vec3(x, y, z);
  }

  public float Lerp(float a, float b, float t)
  {
    return a + (b - a) * t;
  }
}
