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
  static int score = 0;

  public Entity Score;

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

  private Transform birdTransform;
  private Transform topTransform;
  private Transform bottomTransform;

  private TextComponent scoreText;

  public override void Init()
  {
    birdTransform = Bird.GetComponent<Transform>();
    topTransform = TopPart.GetComponent<Transform>();
    bottomTransform = BottomPart.GetComponent<Transform>();

    scoreText = Score.GetComponent<TextComponent>();

    Events.Subscribe<PipeHitPlayer>(self, HitPipe);
    currentSpeed = moveSpeed;

    topStartingPos = topTransform.GetPosition();
    bottomStartingPos = bottomTransform.GetPosition();
  }

  public override void Update()
  {
    Vec3 birdPos = birdTransform.GetPosition();
    Vec3 topPos = topTransform.GetPosition();
    Vec3 bottomPos = bottomTransform.GetPosition();

    Vec3 birdScale = birdTransform.GetScale();
    Vec3 topScale = topTransform.GetScale();
    Vec3 bottomScale = bottomTransform.GetScale();

    if (NITELITE.Input.GetKeyTriggered(Keys.SPACE) && !move) move = true;

    if (move)
    {
      //pipe movement and looping
      topPos.x -= dt * currentSpeed;
      bottomPos.x -= dt * currentSpeed;
      topTransform.SetPosition(topPos);
      bottomTransform.SetPosition(bottomPos);

      //tp wrap
      if (bottomPos.x <= -5)
      {
        //randomly position the pipes y
        int xInt = rng.Next(1500);
        float ran = (float)xInt / 1000f;
        if (rng.Next(2) == 0) ran = -ran;

        topPos.y = topStartingPos.y + ran;
        bottomPos.y = bottomStartingPos.y + ran;

        topPos.x = 5;
        bottomPos.x = 5;

        topTransform.SetPosition(topPos);
        bottomTransform.SetPosition(bottomPos);

        score++;
        scoreText.Text = score.ToString();
      }

      //speed increase over time
      currentSpeed += dt * dtMult;
    }

    //super real collision checks
    if ((birdPos.x <= bottomPos.x + (bottomScale.x / 2)) && (birdPos.x >= bottomPos.x - (bottomScale.x / 2)))
    {
      if ((birdPos.y <= bottomPos.y + (bottomScale.y / 2)) && (birdPos.y >= bottomPos.y - (bottomScale.y / 2)))
      {
        //KILL HIM
        birdPos.y = -8f;
        birdTransform.SetPosition(birdPos);

        PipeHitPlayer pipeHitPlayer;
        pipeHitPlayer.hit = true;
        Events.Signal<PipeHitPlayer>(ref pipeHitPlayer);
      }
    }

    if ((birdPos.x <= topPos.x + (topScale.x / 2)) && (birdPos.x >= topPos.x - (topScale.x / 2)))
    {
      NL_INFO("Bird X Collision");
      if ((birdPos.y <= topPos.y + (topScale.y / 2)) && (birdPos.y >= topPos.y - (topScale.y / 2)))
      {
        //KILL HIM
        birdPos.y = -8f;
        birdTransform.SetPosition(birdPos);

        PipeHitPlayer pipeHitPlayer;
        pipeHitPlayer.hit = true;
        Events.Signal<PipeHitPlayer>(ref pipeHitPlayer);
      }
    }

    //murder
    if (birdPos.y <= -4f)
    {
      PipeHitPlayer pipeHitPlayer;
      pipeHitPlayer.hit = true;
      Events.Signal<PipeHitPlayer>(ref pipeHitPlayer);
    }
  }

  public void HitPipe(ref PipeHitPlayer yes)
  {
    if (yes.hit)
    {
      move = false;
      currentSpeed = moveSpeed;

      topTransform.SetPosition(topStartingPos);
      bottomTransform.SetPosition(bottomStartingPos);
      score = 0;
      scoreText.Text = score.ToString();
    }
  }

  public override void Exit()
  {

  }
}
