/**
/ file:   Camera.cs
/ author: jared
/ date:   January 21, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  First person camera controller
**/
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime;
using Microsoft.VisualBasic;
using NITELITE;

public struct CameraController
{
  public float MouseSensitivityX;
  public float MouseSensitivityY;
  public float MinPitch;
  public float MaxPitch;
  public float LerpSpeed;
  public float maxRotationFrame;
}

public class Camera : NL_Script
{
  public float MoveSpeed = 5f;

  //Idle Bobbing
  private Vec3 InitialPos;
  public double BobFrequency = 0.25;
  public double BobAmplitude = 0.1;
  private double Timer = Math.PI / 2;
  public float LerpValue;
  public float InitialLerp = 0.25f;
  public float MaxLerp = 10f;

  //Camera Controller
  public CameraController CamCon;
  Vec3 NewCamRot = new Vec3();
  private float lastX = 0;

  public override void Init()
  {
    NewCamRot = self.GetComponent<CameraComponent>().rotation;

    InitialPos = self.GetComponent<Transform>().position;
    LerpValue = InitialLerp;
  }

  public override void Update()
  {
    IdleBobbing();
    CameraController();
  }

  public override void Exit()
  {

  }

  public void IdleBobbing()
  {
    //get components
    ref Transform transform = ref self.GetComponent<Transform>();

    //fun movement
    if (NITELITE.Input.GetKeyPressed(Keys.W))
    {
      //Return to parent pos
      float LV = Lerp(LerpValue, MaxLerp, dt);
      LerpValue = LV;

      transform.position = LerpVec3(transform.position, InitialPos, dt * LerpValue);

    }
    else
    {
      LerpValue = InitialLerp;

      Timer += BobFrequency * dt;
      Vec3 NewPos = new Vec3((float)(Math.Sin(Timer * Math.PI * 2) * BobAmplitude * 0.5), (float)(Math.Cos(5 * Timer * Math.PI) * BobAmplitude), transform.position.z);

      transform.position = LerpVec3(transform.position, NewPos, dt * 2f);
    }
  }

  public void CameraController()
  {
    //Rotation based on mouse input
    NewCamRot += new Vec3(Math.Clamp(NITELITE.Input.MouseDelta.x * CamCon.MouseSensitivityX, -CamCon.maxRotationFrame, CamCon.maxRotationFrame), 
                          -NITELITE.Input.MouseDelta.y * CamCon.MouseSensitivityY, 0f);

    //Check for looping and adjust to closer side for lerping
    float rotationX = self.GetComponent<CameraComponent>().rotation.x;
    if (Math.Abs(lastX - rotationX) > 350f)
    {
      if (Math.Abs(NewCamRot.x - rotationX) > Math.Abs((NewCamRot.x - 360) - rotationX))
      {
        NewCamRot.x -= 360;
      }
      if (Math.Abs(NewCamRot.x - rotationX) > Math.Abs((NewCamRot.x + 360) - rotationX))
      {
        NewCamRot.x += 360f;
      }
    }

    lastX = rotationX;

    //Lerp rotation and clamp pitch
    Vec3 CurrentRotation = LerpVec3(self.GetComponent<CameraComponent>().rotation, NewCamRot, dt * CamCon.LerpSpeed);
    float ClampedPitch = Math.Clamp(CurrentRotation.y, CamCon.MinPitch, CamCon.MaxPitch);
    self.GetComponent<CameraComponent>().rotation = new Vec3(CurrentRotation.x, ClampedPitch, CurrentRotation.z);
  }

  public Vec3 LerpVec3(Vec3 startPos, Vec3 endPos, float t)
  {
    float x = Lerp(startPos.x, endPos.x, t);
    float y = Lerp(startPos.y, endPos.y, t);
    float z = Lerp(startPos.z, endPos.z, t);
    return new Vec3(x, y, z);
  }

  public float Lerp(float a, float b, float t)
  {
    return a + (b - a) * t;
  }
}
