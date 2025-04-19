/**
/ file:   ScrollingCredits.cs
/ author: taylor.cadwallader
/ date:   April 18, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  Auto-scrolling for the game credits.
**/
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Numerics;
using NITELITE;

public class ScrollingCredits : NL_Script
{
  public float ScrollSpeed = 1f;
  public Entity Console;

  private Transform transform;
  private Console consoleScript;

  public override void Init()
  {
    transform = self.GetComponent<Transform>();
    consoleScript = Console.GetComponent<Console>();
  }

  public override void Update()
  {
    if (!consoleScript.MinigameActive) return;

    Vec3 myPos = transform.GetPosition();
    transform.SetPosition(new Vec3(myPos.x, myPos.y +=1 * ScrollSpeed *  dt, myPos.z));
  }

  public override void Exit()
  {

  }
}
