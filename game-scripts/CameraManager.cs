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

public class CameraManager : NL_Script
{
  public CameraComponent Camera;

  public override void Init()
  {
    Camera.isMainCamera = true;
  }

  public override void Update()
  {

  }

  public override void Exit()
  {

  }
}
