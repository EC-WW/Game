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

public struct PipeHitPlayer : Event
{
  public bool hit;
}

public class FLAPPY_Pipe : NL_Script
{
  public Entity Bird;

  public Entity TopPart;
  public Entity BottomPart;

  public float pipeOffX;
  public float pipeOffY;

  private float moveSpeed = 1.5f;
  private float currentSpeed;
  private bool move = false;

  private Vec3 topStartingPos;
  private Vec3 bottomStartingPos;

  Random rng = new Random();

  public float dtMult = 0.075f;

  public override void Init()
  {
    Events.Subscribe<PipeHitPlayer>(self, HitPipe);
    currentSpeed = moveSpeed;

    ref Transform birdTransform = ref Bird.GetComponent<Transform>();
    ref Transform topTransform = ref TopPart.GetComponent<Transform>();
    ref Transform bottomTransform = ref BottomPart.GetComponent<Transform>();
    topStartingPos = topTransform.position;
    bottomStartingPos = bottomTransform.position;
  }

  public override void Update()
  {
    ref Transform birdTransform = ref Bird.GetComponent<Transform>();
    ref Transform topTransform = ref TopPart.GetComponent<Transform>();
    ref Transform bottomTransform = ref BottomPart.GetComponent<Transform>();

    if (NITELITE.Input.GetKeyTriggered(Keys.SPACE) && !move) move = true;

    if (move)
    {
      //pipe movement and looping
      topTransform.position.x -= dt * currentSpeed;
      bottomTransform.position.x -= dt * currentSpeed;
      //tp wrap
      if (bottomTransform.position.x <= -5)
      {
        topTransform.position.x = 8;
        bottomTransform.position.x = 8;

        //randomly position the pipes y
        int xInt = rng.Next(1500);
        float ran = (float)xInt / 1000f;
        if (rng.Next(2) == 0) ran = -ran;

        topTransform.position.y = topStartingPos.y + ran;
        bottomTransform.position.y = bottomStartingPos.y + ran;
      }

      //speed increase over time
      currentSpeed += dt * dtMult;
    }

    //super real collision checks
    if (birdTransform.position.x >= bottomTransform.position.x - (pipeOffX * 2) && birdTransform.position.x <= bottomTransform.position.x - pipeOffX)
    {
      if(birdTransform.position.y <= bottomTransform.position.y + pipeOffY || birdTransform.position.y >= topTransform.position.y - (pipeOffY * 2.2f))
      {
        //KILL HIM
        birdTransform.position.y = -8f;

        PipeHitPlayer pipeHitPlayer;
        pipeHitPlayer.hit = true;
        Events.Signal<PipeHitPlayer>(ref pipeHitPlayer);
      }
    }

    //murder
    if(birdTransform.position.y <= -7f)
    {
      PipeHitPlayer pipeHitPlayer;
      pipeHitPlayer.hit = true;
      Events.Signal<PipeHitPlayer>(ref pipeHitPlayer);
    }
  }

  public void HitPipe(ref PipeHitPlayer yes)
  {
    ref Transform topTransform = ref TopPart.GetComponent<Transform>();
    ref Transform bottomTransform = ref BottomPart.GetComponent<Transform>();

    if (yes.hit)
    {
      move = false;
      currentSpeed = moveSpeed;

      topTransform.position = topStartingPos;
      bottomTransform.position = bottomStartingPos;
    }
  }

  public override void Exit()
  {

  }
}
