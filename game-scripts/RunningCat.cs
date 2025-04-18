/**
/ file:   ZombieManager.cs
/ author: aditya.prakash
/ date:   April 16, 2025
/ Copyright (c) 2025 DigiPen (USA) Corporation. 
/ 
/ brief:  Example script for the NITELITE engine, uses several of its features
**/
using System;
using System.Collections.Generic;
using NITELITE;

public class RunningCat : NL_Script
{
  public Entity cat;

  private float timer = 0f;

  public override void Init()
  {
  }

  public override void Update()
  {
    timer += dt;

    // Ellipse parameters
    float cx = -2.75f;
    float cz = 2f;
    float rx = 0.25f;
    float rz = 2f;

    // Position on the ellipse
    float x = cx + rx * MathF.Cos(timer);
    float z = cz + rz * MathF.Sin(timer);

    Transform tf = cat.GetComponent<Transform>();
    Vec3 pos = tf.GetPosition();
    Vec3 newPos = new Vec3(x, pos.y, z);
    tf.SetPosition(newPos);

    // Compute direction of movement (derivative of ellipse parametric)
    float dx = -rx * MathF.Sin(timer);
    float dz = rz * MathF.Cos(timer);

    // Calculate yaw angle in radians (rotation around y-axis)
    float yaw = MathF.Atan2(dx, dz); // assuming +z is forward

    // Set rotation: pitch and roll stay the same
    tf.SetRotation(new Vec3(0f, yaw, 0f));
  }

  public override void Exit()
  {
  }
}