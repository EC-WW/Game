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

public class TaylorTest : NL_Script
{
  private Transform hall;

  public override void Init()
  {

  }

  public override void Update()
  {
    if (NITELITE.Input.GetKeyTriggered(Keys.P))
    {
      //Scene.LoadScene("assets/scenes/Suika.nlscene");
      Entity newHall = Scene.LoadPrefab("assets/prefabs/Hallway.nlprefab");
      hall = newHall.GetComponent<Transform>();

      NL_INFO("Spawned hall!");
    }
  }

  public override void Exit()
  {

  }
}
