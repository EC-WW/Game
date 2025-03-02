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
  public Vec3 DepthOffset = new Vec3(1f, 0f, 1f);

  public override void Init()
  {
    
  }

  public override void Update()
  {
    ref Transform transform = ref self.GetComponent<Transform>();

    Vec3 CamPos = Camera.GetComponent<Transform>().position;
    Vec3 CamDir = Camera.GetComponent<CameraComponent>().facingDirection;

    Vec3 NewPos = CamPos + CamDir * DepthOffset.z + new Vec3(DepthOffset.x, DepthOffset.y, 0f);

    transform.position = NewPos;
    

  }

  public override void Exit()
  {

  }
}
