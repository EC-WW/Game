/**
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
  public float LerpSpeed;

  private Vec3 DepthOffset = new Vec3(1f, 0.6f, 0.1f);
  public float tempDepthX = 1f;
  public float tempDepthY = -0.3f;
  public float tempDepthZ = 0.6f;

  public override void Init()
  {
    
  }

  public override void Update()
  {
    ref Transform transform = ref self.GetComponent<Transform>();
    Vec3 camPos = Camera.GetComponent<Transform>().position;

    //get the camera's forward direction and calculate the right and up
    Vec3 camForward = Camera.GetComponent<CameraComponent>().facingDirection;

    Vec3 camRight = new Vec3(camForward.z, 0, -camForward.x);
    //normalize right
    camRight.Normalize();

    Vec3 camUp = CrossProduct(camRight, camForward);
    //normalize up
    camUp.Normalize();


    //apply the depth offset
    Vec3 targetPos = camPos +
      (camForward * tempDepthX) +
      (camUp * -tempDepthY) +
      (camRight * tempDepthZ);


    //set the position of the hands
    transform.position = targetPos;

    //set the rotations of the hands
    //using camera's x and y rotations cuz their flipped for some silly reason
    transform.rotation.y = DegreesToRadians(Camera.GetComponent<CameraComponent>().rotation.x) - DegreesToRadians(Camera.GetComponent<Transform>().rotation.x);
    //needs to be flipped again with the -1 so it correctly rotates while facing outward
    transform.rotation.x = (DegreesToRadians(Camera.GetComponent<CameraComponent>().rotation.y) - DegreesToRadians(Camera.GetComponent<Transform>().rotation.y)) * -1;
  }

  public override void Exit()
  {

  }

  private float DegreesToRadians(float degrees)
  {
    return degrees * ((float)Math.PI) / 180f;
  }

  private float RadiansToDegrees(float radians)
  {
    return radians * 180f / ((float)Math.PI);
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
