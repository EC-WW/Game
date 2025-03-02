/**
/ file:   FLAPPY_Bird.cs
/ author: taylor.cadwallader
/ date:   February 28, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief: FLAPPYBIRD bird controller
**/
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Microsoft.VisualBasic;
using NITELITE;

public class FLAPPY_Bird : NL_Script
{
  public float UpwardTime = 1f;
  public float UpwardForce = 6f;
  public float Gravity = 3.5f;

  private float lowestFallTime = -3f;
  private float currentTime = 0f;
  private float currentForce = 0f;

  private Vec3 startPos = Vec3.Zero;

  public override void Init()
  {
    ref Transform transform = ref self.GetComponent<Transform>();
    startPos = transform.position;
  }

  public override void Update()
  {
    ref Transform transform = ref self.GetComponent<Transform>();

    if (NITELITE.Input.GetKeyTriggered(Keys.SPACE) && transform.position.y < 8)
    {
      currentTime = UpwardTime;
      currentForce = UpwardForce;
    }

    BirdMovement();
    LoseCondition();
  }

  private void BirdMovement()
  {
    ref Transform transform = ref self.GetComponent<Transform>();

    Vec3 movement = new Vec3(0, 1, 0) * dt * currentTime * currentForce;
    transform.position += movement;

    if(currentTime > lowestFallTime)
      currentTime -= dt * Gravity;
  }

  private void LoseCondition()
  {
    ref Transform transform = ref self.GetComponent<Transform>();

    if (transform.position.y <= -8)
    {
      currentTime = 0f;
      currentForce = 0f;
      transform.position = startPos;
    }
  }

  public override void Exit()
  {

  }
}
