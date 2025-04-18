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

public class FollowEntity : NL_Script
{
  public Entity EntToFollow;

  private Transform entTransform;
  private Transform myTransform;

  public override void Init()
  {
    entTransform = EntToFollow.GetComponent<Transform>();
    myTransform = self.GetComponent<Transform>();
  }

  public override void Update()
  {
    Vec3 entPos = entTransform.GetPosition();
    Vec3 followPos = new Vec3(entPos.x, 0, entPos.z);
    myTransform.SetPosition(followPos);
  }

  public override void Exit()
  {

  }
}
