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

  public override void Init()
  {
    ref Transform transform = ref self.GetComponent<Transform>();
    startPosition = transform.position;
  }

  public override void Update()
  {
    //initially launch the ball
    if (!move) HandleInitialLaunch();

    if (NITELITE.Input.GetKeyPressed(Keys.R))
    {
      move = false;
      ref Transform transform = ref self.GetComponent<Transform>();
      transform.position = startPosition;
    }

    //handle bounces and scores
    HandleMovement();
    HandleScoring();
  }

  private void HandleScoring()
  {
    ref Transform transform = ref self.GetComponent<Transform>();

    //RIGHT player should score
    if (transform.position.x >= 12.5)
    {
      NL_INFO("Right player scored.");
      move = false;
      transform.position = startPosition;

      //ref TextComponent text = ref SpaceText.GetComponent<TextComponent>();
      //text.Text = "'Space' to Start";
    }

    //LEFT player should score
    if (transform.position.x <= -13)
    {
      NL_INFO("Left player scored.");
      move = false;
      transform.position = startPosition;

      //ref TextComponent text = ref SpaceText.GetComponent<TextComponent>();
      //text.Text = "'Space' to Start";
    }
  }

  private void HandleMovement()
  {
    ref Transform transform = ref self.GetComponent<Transform>();
    ref Transform leftPadTransform = ref LeftPaddle.GetComponent<Transform>();
    ref Transform rightPadTransform = ref RightPaddle.GetComponent<Transform>();

    //bouncing off of left paddle
    if (transform.position.x >= leftPadTransform.position.x - paddleOffX)
    {
      if (transform.position.y <= leftPadTransform.position.y + paddleOffY &&
      transform.position.y >= leftPadTransform.position.y - paddleOffY)
      {
        //set ball position to hopefully avoid repeats
        moveDirection.x = -moveDirection.x;
        transform.position.x = leftPadTransform.position.x - paddleOffX - 0.1f;

        //faster
        currentSpeed *= SpeedMult;
      }
    }

    //bouncing off of right paddle
    if (transform.position.x <= rightPadTransform.position.x + paddleOffX)
    {
      if (transform.position.y <= rightPadTransform.position.y + paddleOffY &&
      transform.position.y >= rightPadTransform.position.y - paddleOffY)
      {
        //set ball position to hopefully avoid repeats
        moveDirection.x = -moveDirection.x;
        transform.position.x = rightPadTransform.position.x + paddleOffX + 0.1f;

        //faster
        currentSpeed *= SpeedMult;
      }
    }

    // Bouncing off top and bottom of the screen
    if (transform.position.y <= -6.5)
    {
      moveDirection.y = -moveDirection.y;
      transform.position.y = -6.5f + 0.1f;
    }
    if (transform.position.y >= 5.5)
    {
      moveDirection.y = -moveDirection.y;
      transform.position.y = 5.5f - 0.1f;
    }

    //ball movement
    if (!move) return;
    moveDirection.Normalize();
    Vec3 movement = moveDirection * dt * currentSpeed;
    transform.position += movement;
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
