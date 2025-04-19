/**
/ file:   Console.cs
/ author: taylor.cadwallader
/ date:   April 18, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  Console that is used to trigger minigame experiences. This script is an abomination, viewer discretion is advised.
**/
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Numerics;
using NITELITE;

public class Console : NL_Script
{
  public Entity PlayerEntity;
  public Entity CamEntity;
  public Entity TargetCamera;
  public TextComponent TextPrompt;
  public float DistanceCheck = 3f;

  public float CamMoveSpeed = 0.8f;
  public float CamRotSpeed = 2f;

  public TextComponent ReturnText;

  private Transform transform;
  private Vec3 myPos;
  private FirstPersonCamera camScript;
  private CameraComponent camComponent;
  private Transform camTransform;
  private Vec3 camPos;

  private FirstPersonController_NEW playerScript;

  private CameraComponent targetCamComponent;
  private Transform targetCamTransform;

  private bool canTrigger = false;
  private float dtMult = 1f;
  private bool camSequence = false;
  private bool triggered = false;

  private Vec3 originalFaceDirection;
  private Vec3 originalPos;
  private Vec3 targetDirection;
  private Vec3 targetPosition;

  private bool followTargetCam = false;

  public bool MinigameActive = false;

  private bool useTargetCamPosition = false;

  public override void Init()
  {
    transform = self.GetComponent<Transform>();
    camTransform = CamEntity.GetComponent<Transform>();
    camScript = CamEntity.GetComponent<FirstPersonCamera>();
    camComponent = CamEntity.GetComponent<CameraComponent>();

    targetCamTransform = TargetCamera.GetComponent<Transform>();
    targetCamComponent = TargetCamera.GetComponent<CameraComponent>();

    playerScript = PlayerEntity.GetComponent<FirstPersonController_NEW>();
  }

  public override void Update()
  {
    myPos = transform.GetGlobalPosition();
    camPos = camTransform.GetPosition();

    TriggerCheck();
    HandleTransitions();
    HandleCameraFollowing();
  }

  private void TriggerCheck()
  {
    Vec3 difference = new Vec3(myPos.x - camPos.x, 0, myPos.z - camPos.z);
    if (difference.Magnitude() <= DistanceCheck)
    {
      if (!canTrigger)
      {
        TextPrompt.Text = "E";
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

  private void HandleTransitions()
  {
    Vec3 targetCamPosition = targetCamTransform.GetPosition();

    //trigger the chain of events
    if (!triggered && canTrigger && NITELITE.Input.GetKeyTriggered(Keys.E))
    {
      originalFaceDirection = camComponent.facingDirection;
      originalPos = camTransform.GetPosition();

      targetDirection = targetCamComponent.facingDirection;
      //targetPosition = targetCamPosition;
      useTargetCamPosition = true;

      ReturnText.Text = "'R' to Return";

      dtMult = 1;
      camScript.StopEverything = true;
      playerScript.StopEverything = true;
      camSequence = true;
      triggered = true;
    }

    if (triggered && NITELITE.Input.GetKeyTriggered(Keys.R))
    {
      followTargetCam = false;

      targetDirection = originalFaceDirection;

      useTargetCamPosition = false;
      targetPosition = originalPos;

      ReturnText.Text = "";

      dtMult = 1;
      camSequence = true;
    }

    if (camSequence)
    {
      Vec3 diffA;
      if (!useTargetCamPosition)
      {
        diffA = targetPosition - camPos;
      }
      else
      {
        diffA = targetCamPosition - camPos;
      }

      if (diffA.Magnitude() > 0.1f)
      {
        if (!useTargetCamPosition)
        {
          camTransform.SetPosition(LerpVec3(camPos, targetPosition, dt * CamMoveSpeed * dtMult));
        }
        else
        {
          camTransform.SetPosition(LerpVec3(camPos, targetCamPosition, dt * CamMoveSpeed * dtMult));
        }

        camComponent.facingDirection = LerpVec3(camComponent.facingDirection, targetDirection, dt * CamRotSpeed * dtMult);

        dtMult += dt;
      }
      else
      {
        dtMult = 1f;
        camSequence = false;

        if(SameVector(targetPosition, originalPos))
        {
          camScript.StopEverything = false;
          playerScript.StopEverything = false;
          triggered = false;
        }
        else
        {
          followTargetCam = true;

          MinigameActive = true;
        }
      }
    }
  }

  private void HandleCameraFollowing()
  {
    if (!followTargetCam) return;

    camTransform.SetPosition(targetCamTransform.GetGlobalPosition());
    camComponent.facingDirection = targetCamComponent.facingDirection;
  }

  private bool SameVector(Vec3 a, Vec3 b)
  {
    return a.x == b.x && a.y == b.y && a.z == b.z;
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
