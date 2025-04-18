/**
/ file:   FirstPersonCamera.cs
/ author: taylor.cadwallader
/ date:   April 16, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  Camera controller for a first person player controller.
**/
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Numerics;
using NITELITE;

public class FirstPersonCamera : NL_Script
{
  //temporary transform to make the camera move with the player without parenting
  public Transform FollowPoint;
  public Transform Player;

  //Idle Bobbing
  public float LerpValue;
  public float InitialLerp = 0.25f;
  public float MaxLerp = 10f;
  public double BobFrequency = 0.25;
  public double BobAmplitude = 0.1;

  private Vec3 initialPos;
  private double bobTimer = Math.PI / 2;

  //Camera Controller
  public float MouseSensitivityX = 0.3f;
  public float MouseSensitivityY = 0.3f;
  public float MinPitch = -60f;
  public float MaxPitch = 70f;
  public float LerpSpeed = 40f;
  public float maxRotationFrame = 50f;


  public bool StopEverything = false;

  Vec3 newCamRot = Vec3.Zero;
  private float lastX = 0;

  private Transform transform;
  private CameraComponent camera;


  //debug
  private bool canLook = true;

  public override void Init()
  {
    transform = self.GetComponent<Transform>();
    camera = self.GetComponent<CameraComponent>();

    initialPos = transform.GetPosition();
    newCamRot = camera.rotation;

    LerpValue = InitialLerp;
  }

  public override void Update()
  {
    if (StopEverything) return;

    //IdleBobbing();
    CameraLook();
    CameraFollow();

    //freeze ability to look for debugging
    if (NITELITE.Input.GetKeyTriggered(Keys.O))
      canLook = !canLook;
  }

  private void IdleBobbing()
  {
    //fun movement
    if (NITELITE.Input.GetKeyPressed(Keys.W))
    {
      //Return to parent pos
      float LV = Lerp(LerpValue, MaxLerp, dt);
      LerpValue = LV;

      transform.SetPosition(LerpVec3(transform.GetPosition(), initialPos, dt * LerpValue));
    }
    else
    {
      LerpValue = InitialLerp;
      bobTimer += BobFrequency * dt;

      Vec3 currentPos = transform.GetPosition();
      Vec3 newPos = new Vec3((float)(Math.Sin(bobTimer * Math.PI * 2) * BobAmplitude * 0.5), 
        (float)(Math.Cos(5 * bobTimer * Math.PI) * BobAmplitude), currentPos.z);

      transform.SetPosition(LerpVec3(transform.GetPosition(), newPos, dt * 2f));
    }
  }

  private void CameraLook()
  {
    if (!canLook) return;

    //set rotation based on mouse input
    newCamRot += new Vec3(Math.Clamp(NITELITE.Input.MouseDelta.x * MouseSensitivityX, -maxRotationFrame, maxRotationFrame),
                          -NITELITE.Input.MouseDelta.y * MouseSensitivityY, 0f);

    float rotationX = camera.rotation.x;

    //fix infinite spin bug
    if (Math.Abs(lastX - rotationX) > 350f)
    {
      if (Math.Abs(newCamRot.x - rotationX) > Math.Abs((newCamRot.x - 360) - rotationX))
        newCamRot.x -= 360;

      if (Math.Abs(newCamRot.x - rotationX) > Math.Abs((newCamRot.x + 360) - rotationX))
        newCamRot.x += 360f;
    }

    //store the last x rotation value
    lastX = rotationX;

    //Lerp rotation and clamp pitch
    Vec3 CurrentRotation = LerpVec3(camera.rotation, newCamRot, dt * LerpSpeed);
    float ClampedPitch = Math.Clamp(CurrentRotation.y, MinPitch, MaxPitch);
    camera.rotation = new Vec3(CurrentRotation.x, ClampedPitch, CurrentRotation.z);
  }

  private void CameraFollow()
  {
    Vec3 playerPos = Player.GetPosition();
    Vec3 followPos = FollowPoint.GetPosition();
    //follow point is a child object of the player that essentially is being used as just an offset
    transform.SetPosition(playerPos + followPos);
  }

  public override void Exit()
  {

  }

  

  #region Math Functions
  private Vec3 LerpVec3(Vec3 startPos, Vec3 endPos, float t)
  {
    float x = Lerp(startPos.x, endPos.x, t);
    float y = Lerp(startPos.y, endPos.y, t);
    float z = Lerp(startPos.z, endPos.z, t);
    return new Vec3(x, y, z);
  }

  private float Lerp(float a, float b, float t)
  {
    return a + (b - a) * t;
  }
  #endregion
}
