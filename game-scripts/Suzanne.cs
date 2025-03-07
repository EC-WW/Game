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

  public override void Init()
  {
    EndPos = new Vec3(self.GetComponent<Transform>().position.x, EndY, self.GetComponent<Transform>().position.z);
  }

  public override void Update()
  {
    if (NITELITE.Input.GetKeyTriggered(Keys.MOUSE_LEFT))
    {
      Clicked = true;
    }

    if (Clicked)
    {
      self.GetComponent<Transform>().position = LerpVec3(self.GetComponent<Transform>().position, EndPos, dt);
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
