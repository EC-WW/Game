/**
/ file:   PONG_Paddle.cs
/ author: taylor.cadwallader
/ date:   February 27, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief: PONG paddle controller
**/
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime;
using System.Security.Cryptography;
using Microsoft.VisualBasic;
using NITELITE;

public class PONG_Paddle : NL_Script
{
  public float MoveSpeed = 5f;
  public float maxYPos = 0f;
  public float minYPos = 0f;

  public bool IsLeftPaddle = true;

  public override void Init()
  {

  }

  public override void Update()
  {
    ref Transform transform = ref self.GetComponent<Transform>();

    if (IsLeftPaddle)
    {
      if (NITELITE.Input.GetKeyPressed(Keys.W) && transform.position.y < maxYPos)
      {
        Vec3 movement = new Vec3(0, 1, 0) * dt * MoveSpeed;
        transform.position += movement;
      }
      if (NITELITE.Input.GetKeyPressed(Keys.S) && transform.position.y > minYPos)
      {
        Vec3 movement = new Vec3(0, -1, 0) * dt * MoveSpeed;
        transform.position += movement;
      }
    }
    else
    {
      if (NITELITE.Input.GetKeyPressed(Keys.I) && transform.position.y < maxYPos)
      {
        Vec3 movement = new Vec3(0, 1, 0) * dt * MoveSpeed;
        transform.position += movement;
      }
      if (NITELITE.Input.GetKeyPressed(Keys.K) && transform.position.y > minYPos)
      {
        Vec3 movement = new Vec3(0, -1, 0) * dt * MoveSpeed;
        transform.position += movement;
      }
    }
  }

  public override void Exit()
  {

  }
}
