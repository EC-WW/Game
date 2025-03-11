/**
/ file:   Camera.cs
/ author: jared
/ date:   January 21, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  First person camera controller
**/
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime;
using Microsoft.VisualBasic;
using NITELITE;

public class Suzanne : NL_Script
{
  private bool Clicked = false;
  private Vec3 EndPos;
  public float EndY = 0f;

  private Transform transform;

  public override void Init()
  {
    transform = self.GetComponent<Transform>();

    Vec3 pos = transform.GetPosition();
    EndPos = new Vec3(pos.x, EndY, pos.z);
  }

  public override void Update()
  {
    Vec3 pos = transform.GetPosition();

    if (NITELITE.Input.GetKeyTriggered(Keys.MOUSE_LEFT))
    {
      Clicked = true;
    }

    if (Clicked)
    {
      Vec3 newPos = LerpVec3(pos, EndPos, dt);
      transform.SetPosition(newPos);
    }
  }

  public override void Exit()
  {

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

  private float RadiansToDegrees(float radians)
  {
    return radians * ((float)Math.PI) / 180f;
  }
}
