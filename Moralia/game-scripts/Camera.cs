/**
/ file:   CameraTest.cs
/ author: jared
/ date:   January 21, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  Testing C# with the camera
**/
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime;
using Microsoft.VisualBasic;
using NITELITE;

public class Camera : NL_Script
{
  Entity myent;

  public Vec3 InitialPos;
  public double BobFrequency = 0.25;
  public double BobAmplitude = 0.1;
  private double Timer = Math.PI / 2;
  public float LerpValue;
  public float InitialLerp = 0.25f;
  public float MaxLerp = 10f;

  Vec3 NewCamRot = new Vec3();
  public float MouseSensitivityX = 0.1f;
  public float MouseSensitivityY = 0.1f;
  public float CameraLerp = 40;
  public float matRotationFrame = 50f;
  float lastX = 0;

  public override void Init()
  {
    NewCamRot = self.GetComponent<CameraComponent>().rotation;

    InitialPos = self.GetComponent<Transform>().position;
    LerpValue = InitialLerp;
  }

  public override void Update()
  {
    //IdleBobbing();
    CameraController();
    PitchClamp();
  }

  public override void Exit()
  {
    // This is called when the script is unloaded
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
    NewCamRot += new Vec3(Math.Clamp(NITELITE.Input.MouseDelta.x * MouseSensitivityX, -matRotationFrame, matRotationFrame), 
                          -NITELITE.Input.MouseDelta.y * MouseSensitivityY, 0f);

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
    self.GetComponent<CameraComponent>().rotation = LerpVec3(self.GetComponent<CameraComponent>().rotation, NewCamRot, dt * CameraLerp);
  }

  public void PitchClamp()
  {
    Vec3 CurrentRotation = self.GetComponent<CameraComponent>().rotation;

    float ClampedPitch = Math.Clamp(CurrentRotation.y, -60f, 70f);

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
