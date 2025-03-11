/**
/ file:   PONG_Ball.cs
/ author: taylor.cadwallader
/ date:   February 27, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief: PONG ball functionality
**/
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime;
using System.Security.Cryptography;
using Microsoft.VisualBasic;
using NITELITE;

public class PONG_Ball : NL_Script
{
  public float DefaultSpeed = 5f;
  public float SpeedMult = 1.07f;
  private float currentSpeed;
  //public Entity SpaceText;

  public float paddleOffX;
  public float paddleOffY;

  private Vec3 startPosition = Vec3.Zero;

  private Vec3 moveDirection = Vec3.Zero;
  private bool move = false;


  Random rng = new Random();

  public Entity LeftPaddle;
  public Entity RightPaddle;

  private Transform transform;
  private Transform leftPadTransform;
  private Transform rightPadTransform;

  public override void Init()
  {
    transform = self.GetComponent<Transform>();
    leftPadTransform = LeftPaddle.GetComponent<Transform>();
    rightPadTransform = RightPaddle.GetComponent<Transform>();

    startPosition = transform.GetPosition();
  }

  public override void Update()
  {
    //initially launch the ball
    if (!move) HandleInitialLaunch();

    if (NITELITE.Input.GetKeyPressed(Keys.R))
    {
      move = false;
      transform.SetPosition(startPosition);
    }

    //handle bounces and scores
    HandleMovement();
    HandleScoring();
  }

  private void HandleScoring()
  {
    Vec3 pos = transform.GetPosition();

    //RIGHT player should score
    if (pos.x >= 12.5)
    {
      NL_INFO("Right player scored.");
      move = false;
      transform.SetPosition(startPosition);

      //ref TextComponent text = ref SpaceText.GetComponent<TextComponent>();
      //text.Text = "'Space' to Start";
    }

    //LEFT player should score
    if (pos.x <= -13)
    {
      NL_INFO("Left player scored.");
      move = false;
      transform.SetPosition(startPosition);

      //ref TextComponent text = ref SpaceText.GetComponent<TextComponent>();
      //text.Text = "'Space' to Start";
    }
  }

  private void HandleMovement()
  {
    Vec3 pos = transform.GetPosition();
    Vec3 leftPadPos = leftPadTransform.GetPosition();
    Vec3 rightPadPos = rightPadTransform.GetPosition();

    //bouncing off of left paddle
    if (pos.x >= leftPadPos.x - paddleOffX)
    {
      if (pos.y <= leftPadPos.y + paddleOffY &&
      pos.y >= leftPadPos.y - paddleOffY)
      {
        //set ball position to hopefully avoid repeats
        moveDirection.x = -moveDirection.x;
        pos.x = leftPadPos.x - paddleOffX - 0.1f;
        transform.SetPosition(pos);

        //faster
        currentSpeed *= SpeedMult;
      }
    }

    //bouncing off of right paddle
    if (pos.x <= rightPadPos.x + paddleOffX)
    {
      if (pos.y <= rightPadPos.y + paddleOffY &&
      pos.y >= rightPadPos.y - paddleOffY)
      {
        //set ball position to hopefully avoid repeats
        moveDirection.x = -moveDirection.x;
        pos.x = rightPadPos.x + paddleOffX + 0.1f;
        transform.SetPosition(pos);

        //faster
        currentSpeed *= SpeedMult;
      }
    }

    // Bouncing off top and bottom of the screen
    if (pos.y <= -6.5)
    {
      moveDirection.y = -moveDirection.y;
      pos.y = -6.4f;
      transform.SetPosition(pos);
    }
    if (pos.y >= 5.5)
    {
      moveDirection.y = -moveDirection.y;
      pos.y = 5.4f;
      transform.SetPosition(pos);
    }

    //ball movement
    if (!move) return;
    moveDirection.Normalize();
    Vec3 movement = moveDirection * dt * currentSpeed;
    transform.SetPosition(pos + movement);
  }

  private void HandleInitialLaunch()
  {
    //initially launching the ball
    if (NITELITE.Input.GetKeyPressed(Keys.SPACE))
    {
      //ref TextComponent text = ref SpaceText.GetComponent<TextComponent>();
      //text.Text = "a";

      currentSpeed = DefaultSpeed;

      //get random direction in x and y
      int ranIntX = rng.Next(1000);
      float ranX = (float)ranIntX / 1000f;
      if (rng.Next(2) == 0) ranX = -ranX;
      int ranIntY = rng.Next(1000);
      float ranY = (float)ranIntY / 1000f;
      if (rng.Next(2) == 0) ranY = -ranY;

      //set the start direction
      moveDirection = new Vec3(ranX, ranY, 0);
      move = true;
    }
  }

  public override void Exit()
  {

  }
}
