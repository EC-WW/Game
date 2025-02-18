/**
/ file:   Example.cs
/ author: j.m.love
/ date:   November 14, 2024
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  Example script for the NITELITE engine, uses several of its features
**/
using System;
using NITELITE;

public class FirstPersonController : NL_Script
{
  Entity myent;

  

  public override void Init()
  {
    
  }

  public override void Update()
  {
    Console.WriteLine("hi");


  }

  public override void Exit()
  {
    // This is called when the script is unloaded

  }

  public Vec3 LerpPosition(Vec3 startPos, Vec3 endPos, float t)
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
