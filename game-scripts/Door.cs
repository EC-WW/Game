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

public class Door : NL_Script
{
  public string SceneName = "default";
  public string DoorPrompt = "E";
  public string HallColor = "Red";
  public Transform MainParent;
  public Transform DoorParent;
  public TextComponent TextPrompt;
  public float DistanceCheck = 2f;

  public Vec3 OpenedRotation = new Vec3();

  public float CamAMoveSpeed = 0.8f;
  public float CamARotSpeed = 2f;
  public float CamBMoveSpeed = 0.5f;

  public float OpenSpeed = 1f;

  private bool canTrigger = false;

  public Entity CamEntity;
  private Transform camTransform;
  private FirstPersonCamera camScript;
  private CameraComponent camComponent;

  public Transform CamPointA;
  public Transform CamPointB;

  private bool camSequenceA = false;
  private bool openSequence = false;
  private bool transitionSequence = false;
  private float dtMult = 1f;

  private Vec3 camPos;
  private Vec3 parentPos;
  private bool triggered = false;

  public bool NeverEnter = false;

  public override void Init()
  {
    camTransform = CamEntity.GetComponent<Transform>();
    camScript = CamEntity.GetComponent<FirstPersonCamera>();
    camComponent = CamEntity.GetComponent<CameraComponent>();
  }

  public override void Update()
  {
    parentPos = MainParent.GetPosition();
    camPos = camTransform.GetPosition();

    TriggerCheck();
    HandleTransition();
  }

  private void TriggerCheck()
  {
    Vec3 difference = new Vec3(parentPos.x - camPos.x, 0, parentPos.z - camPos.z);
    if (difference.Magnitude() <= DistanceCheck)
    {
      if (!canTrigger)
      {
        TextPrompt.Text = DoorPrompt;
        dtMult = 1f;
        canTrigger = true;
      }
    }
    else
    {
      if (canTrigger)
      {
        TextPrompt.Text = "";
        canTrigger = false;
      }
    }
  }

  private void HandleTransition()
  {
    if (NeverEnter) return;

    Vec3 aPos = CamPointA.GetGlobalPosition();

    //trigger the chain of events
    if (!triggered && canTrigger && NITELITE.Input.GetKeyTriggered(Keys.E))
    {
      camScript.StopEverything = true;
      camSequenceA = true;
      triggered = true;
    }

    #region Part 1
    if (camSequenceA)
    {
      //this needs to be changed to actual global position
      Vec3 diffA = aPos - camPos;
      if (diffA.Magnitude() > 0.1f)
      {
        camTransform.SetPosition(LerpVec3(camPos, aPos, dt * CamAMoveSpeed * dtMult));

        Vec3 lookDir = (parentPos - camPos);
        lookDir.Normalize();

        // Manually invert the direction if needed
        Vec3 flippedDir = new Vec3(lookDir.x, lookDir.y, lookDir.z);
        camComponent.facingDirection = flippedDir;

        dtMult += dt;
      }
      else
      {
        dtMult = 1f;
        camSequenceA = false;
        openSequence = true;

        //spawn hall
        Entity newHall = Scene.LoadPrefab($"assets/prefabs/Hallway({HallColor}).nlprefab");
        Transform hall = newHall.GetComponent<Transform>();
        hall.SetRotation(MainParent.GetRotation());
        hall.SetPosition(parentPos);
      }
    }
    #endregion
    #region Part 2
    if (openSequence)
    {
      Vec3 hingeRot = DoorParent.GetRotation();
      DoorParent.SetRotation(LerpVec3(DoorParent.GetRotation(), OpenedRotation, dt * OpenSpeed * dtMult));
      dtMult += dt;

      Vec3 rotDiff = hingeRot - OpenedRotation;
      if (rotDiff.Magnitude() <= 0.1f)
      {
        openSequence = false;
        transitionSequence = true;
      }
    }
    #endregion
    #region Part 3
    if (transitionSequence)
    {
      Vec3 bPos = CamPointB.GetGlobalPosition();
      Vec3 diffB = bPos - camPos;
      if (diffB.Magnitude() > 0.1f)
      {
        camTransform.SetPosition(LerpVec3(camPos, bPos, dt * CamBMoveSpeed * dtMult));
        dtMult += dt;
      }
      else
      {
        dtMult = 1f;
        transitionSequence = false;
        //finally load scene
        Scene.LoadScene($"assets/scenes/{SceneName}.nlscene");
      }
    }
    #endregion
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
