/**
/ file:   FLAPPY_Pipe.cs
/ author: taylor.cadwallader
/ date:   February 28, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief: FLAPPYBIRD pipe logic
**/
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Microsoft.VisualBasic;
using NITELITE;

public class RotateEnt : NL_Script
{
  public Entity entity;
  public float RotSpeed = 1f;

  private Transform transform;
  public override void Init()
  {
    transform = entity.GetComponent<Transform>();

  }

  public override void Update()
  {
    transform.rotation.y += RotSpeed * dt;
  }

  public override void Exit()
  {

  }
}
