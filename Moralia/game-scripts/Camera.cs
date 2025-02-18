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
  public float MouseSensitivityX = 1f;
  public float MouseSensitivityY = 1f;
  public float CameraLerp = 0.99f;

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
  }

  public override void Exit()
  {
    // This is called when the script is unloaded
  }


  public void IdleBobbing()
  {
    //get components
    ref Transform transform = ref self.GetComponent<Transform>();
    //ref ModelComponent model = ref self.GetComponent<ModelComponent>();
    //ref Particle particle = ref self.GetComponent<Particle>();

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
    NL_INFO(NITELITE.Input.MouseDelta.ToString());
    NewCamRot += new Vec3(NITELITE.Input.MouseDelta.x * MouseSensitivityX, -NITELITE.Input.MouseDelta.y * MouseSensitivityY, 0f);

    self.GetComponent<CameraComponent>().rotation = LerpVec3(self.GetComponent<CameraComponent>().rotation, NewCamRot, dt * CameraLerp);
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
