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

  private Transform transform;

  public override void Init()
  {
    transform = self.GetComponent<Transform>();
  }

  public override void Update()
  {
    Vec3 pos = transform.GetPosition();

    if (IsLeftPaddle)
    {
      if (NITELITE.Input.GetKeyPressed(Keys.W) && pos.y < maxYPos)
      {
        Vec3 movement = new Vec3(0, 1, 0) * dt * MoveSpeed;
        pos += movement;
        transform.SetPosition(pos);
      }
      if (NITELITE.Input.GetKeyPressed(Keys.S) && pos.y > minYPos)
      {
        Vec3 movement = new Vec3(0, -1, 0) * dt * MoveSpeed;
        pos += movement;
        transform.SetPosition(pos);
      }
    }
    else
    {
      if (NITELITE.Input.GetKeyPressed(Keys.I) && pos.y < maxYPos)
      {
        Vec3 movement = new Vec3(0, 1, 0) * dt * MoveSpeed;
        pos += movement;
        transform.SetPosition(pos);
      }
      if (NITELITE.Input.GetKeyPressed(Keys.K) && pos.y > minYPos)
      {
        Vec3 movement = new Vec3(0, -1, 0) * dt * MoveSpeed;
        pos += movement;
        transform.SetPosition(pos);
      }
    }
  }

  public override void Exit()
  {

  }
}
