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

  private Entity newEntity;

  public override void Init()
  {
    Events.Subscribe<PipeHitPlayer>(self, HitPipe);

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

    //if (NITELITE.Input.GetKeyTriggered(Keys.P))
    //{
    //  NL_INFO("SOME SHIT");
    //  newEntity = new Entity();
    //  ref Transform t = ref newEntity.AddComponent<Transform>();
    //  t.position = startPos;

    //  ref ModelComponent m = ref newEntity.AddComponent<ModelComponent>();
    //  m.modelPath = "cube";
    //}

    BirdMovement();

    if (transform.position.y <= -8)
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

    currentTime = 0f;
    currentForce = 0f;
    transform.position = startPos;
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
