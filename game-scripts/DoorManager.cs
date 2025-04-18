/**
/ file:   TaylorTest.cs
/ author: taylor.cadwallader
/ date:   April 16, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  Taylor does what Taylor wants in here.
**/
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Numerics;
using NITELITE;

public class DoorManager : NL_Script
{
  public Entity Camera;
  public Vec3 StartingPoint;
  public Vec3 TargetPoint;
  public Vec3 OpenedRotation;
  public float OpenSpeed = 1f;
  public float CamMoveSpeed = 1f;

  public Transform MainDoor;
  public Transform DoorParent;

  private FirstPersonCamera camScript;
  private Transform camTransform;

  private bool openSequence = false;
  private bool exitSequence = false;
  private float dtMult = 1f;

  private bool end = false;

  public override void Init()
  {
    camScript = Camera.GetComponent<FirstPersonCamera>();
    camScript.StopEverything = true;

    camTransform = Camera.GetComponent<Transform>();
    camTransform.SetPosition(StartingPoint);

    openSequence = true;
  }

  public override void Update()
  {
    //basically stops making any future calls since we can't destroy/disable comps
    if (end) return;

    

    if (openSequence)
    {
      Vec3 hingeRot = DoorParent.GetRotation();
      DoorParent.SetRotation(LerpVec3(DoorParent.GetRotation(), OpenedRotation, dt * OpenSpeed * dtMult));
      dtMult += dt;

      Vec3 rotDiff = hingeRot - OpenedRotation;
      if (rotDiff.Magnitude() <= 0.1f)
      {
        dtMult = 1f;
        openSequence = false;
        exitSequence = true;
      }
    }

    if (exitSequence)
    {
      Vec3 camPos = camTransform.GetPosition();
      Vec3 diff = TargetPoint - camPos;
      if (diff.Magnitude() > 0.1f)
      {
        camTransform.SetPosition(LerpVec3(camPos, TargetPoint, dt * CamMoveSpeed * dtMult));
        dtMult += dt;
      }
      else
      {
        exitSequence = false;
        DoorParent.SetRotation(Vec3.Zero);
        camScript.StopEverything = false;
        end = true;
      }
    }
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
