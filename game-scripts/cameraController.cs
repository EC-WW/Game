/**
/ file:   CameraController.cs
/ author: aditya.prakash
/ date:   April 16, 2025
/ Copyright (c) 2025 DigiPen (USA) Corporation. 
/ 
/ brief:  Example script for the NITELITE engine, uses several of its features
**/
using System;
using NITELITE;

public class CameraController : NL_Script
{
  // Assign your Camera entity in the editor
  public Entity Camera;

  // Movement speed
  public float moveSpeed = 5.0f;
  // Look speed
  public float lookSpeed = 50.0f;

  private Transform camTrans;
  private CameraComponent cameraProps;
  private float yaw;
  private float pitch;

  public override void Init()
  {
    camTrans = Camera.GetComponent<Transform>();
    cameraProps = Camera.GetComponent<CameraComponent>();


    // initialize yaw/pitch from existing rotation
    Vec3 r = cameraProps.rotation;
    yaw = r.x;
    pitch = r.y;
  }

  public override void Update()
  {
    return;

    // —— Rotation —— 
    // LEFT/RIGHT change yaw, UP/DOWN change pitch
    if (NITELITE.Input.GetKeyPressed(Keys.RIGHT)) yaw += lookSpeed * dt;
    if (NITELITE.Input.GetKeyPressed(Keys.LEFT)) yaw -= lookSpeed * dt;
    if (NITELITE.Input.GetKeyPressed(Keys.UP)) pitch += lookSpeed * dt;
    if (NITELITE.Input.GetKeyPressed(Keys.DOWN)) pitch -= lookSpeed * dt;

    // clamp pitch so you don’t flip over
    pitch = Math.Clamp(pitch, -89f, 89f);

    // apply back to camera
    cameraProps.rotation = new Vec3(yaw, pitch, 0f);

    // —— Movement —— 
    // build a flat (y‐zero) forward/right basis from yaw
    float radYaw = yaw * (float)Math.PI / 180f;
    Vec3 forward = new Vec3((float)Math.Sin(radYaw), 0f, (float)Math.Cos(radYaw));
    Vec3 right = new Vec3(forward.z, 0f, -forward.x);
    Vec3 up = new Vec3(0f, 1f, 0f);

    // W/A/S/D/Q/E
    Vec3 move = new Vec3(0f, 0f, 0f);
    if (NITELITE.Input.GetKeyPressed(Keys.W)) move -= forward;  // forward
    if (NITELITE.Input.GetKeyPressed(Keys.S)) move += forward;  // backward
    if (NITELITE.Input.GetKeyPressed(Keys.A)) move += right;    // left
    if (NITELITE.Input.GetKeyPressed(Keys.D)) move -= right;    // right
    if (NITELITE.Input.GetKeyPressed(Keys.Q)) move += up;       // up
    if (NITELITE.Input.GetKeyPressed(Keys.E)) move -= up;       // down

    // normalize & scale by speed and deltaTime
    float len = (float)Math.Sqrt(move.x * move.x + move.y * move.y + move.z * move.z);
    if (len > 0f)
    {
      // divide by len to normalize, then multiply by speed & dt
      Vec3 deltaPos = new Vec3(move.x / len, move.y / len, move.z / len)
                     * (moveSpeed * dt);
      camTrans.SetPosition(camTrans.GetPosition() + deltaPos);
    }
  }

  public override void Exit()
  {
    // nothing to clean up
  }
}
