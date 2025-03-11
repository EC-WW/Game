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

  private Transform transform;

  public override void Init()
  {
    Events.Subscribe<PipeHitPlayer>(self, HitPipe);

    transform = self.GetComponent<Transform>();
    startPos = transform.GetPosition();
  }

  public override void Update()
  {
    Vec3 pos = transform.GetPosition();
    if (NITELITE.Input.GetKeyTriggered(Keys.SPACE) && pos.y < 8)
    {
      currentTime = UpwardTime;
      currentForce = UpwardForce;
    }

    BirdMovement();

    if (pos.y <= -8)
      LoseCondition();
  }

  private void BirdMovement()
  {
    Vec3 pos = transform.GetPosition();
    Vec3 movement = new Vec3(0, 1, 0) * dt * currentTime * currentForce;
    transform.SetPosition(pos + movement);

    if(currentTime > lowestFallTime)
      currentTime -= dt * Gravity;
  }

  private void LoseCondition()
  {
    currentTime = 0f;
    currentForce = 0f;
    transform.SetPosition(startPos);
  }

  public void HitPipe(ref PipeHitPlayer yes)
  {
    if (yes.hit)
    {
      LoseCondition();
    }
  }

  public override void Exit()
  {

  }
}
