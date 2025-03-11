/**
/ file:   FirstPersonController.cs
/ author: taylor.cadwallader
/ date:   February 18, 2024
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  Script that handles player character movement.
**/
using System;
using NITELITE;

public class FirstPersonController : NL_Script
{
  public float WalkSpeed = 7f;
  public float SprintSpeed = 24f;
  public float RotationSpeed = 3f;

  public Entity Camera;
  public Vec3 CamOffset = new Vec3(0f, 2f, 0f);

  private Vec3 inputDirection = new Vec3(0f, 0f, 0f);
  private bool isSprinting = false;

  private Transform transform;
  private Transform camTransform;

  public override void Init()
  {
    transform = self.GetComponent<Transform>();
    camTransform = Camera.GetComponent<Transform>();

    Vec3 camRot = camTransform.GetRotation();
    camRot.x = 180f;
    camTransform.SetRotation(camRot);
  }

  public override void Update()
  {
    Vec3 rot = transform.GetRotation();
    Vec3 camRot = camTransform.GetRotation();

    rot.y = DegreesToRadians(Camera.GetComponent<CameraComponent>().rotation.x) - DegreesToRadians(camRot.x);
    transform.SetRotation(rot);

    camTransform.SetPosition(transform.GetPosition() + CamOffset);


    PlayerInput();
    Movement();
  }

  private void PlayerInput()
  {
    inputDirection = new Vec3(0f, 0f, 0f);

    //update input direction based on input
    if (NITELITE.Input.GetKeyPressed(Keys.W))
    {
      inputDirection.z += 1;
    }
    if (NITELITE.Input.GetKeyPressed(Keys.A))
    {
      inputDirection.x -= 1;
    }
    if (NITELITE.Input.GetKeyPressed(Keys.S))
    {
      inputDirection.z -= 1;
    }
    if (NITELITE.Input.GetKeyPressed(Keys.D))
    {
      inputDirection.x += 1;
    }

    //sprint input check
    if (NITELITE.Input.GetKeyPressed(Keys.LEFT_SHIFT))
    {
      isSprinting = true;
    }
    if (NITELITE.Input.GetKeyReleased(Keys.LEFT_SHIFT))
    {
      isSprinting = false;
    }
  }

  private void Movement()
    {
    Vec3 pos = transform.GetPosition();
    Vec3 rot = transform.GetRotation();

    //normalize input direction to prevent diagonal speed boost
    if (inputDirection.x != 0 || inputDirection.z != 0)
    {
      inputDirection.Normalize();
    }

    //get the forward and right vectors based on rotation
    float radians = transform.GetRotation().y;
    Vec3 forward = new Vec3(MathF.Sin(radians), 0, MathF.Cos(radians));
    Vec3 right = new Vec3(forward.z, 0, -forward.x);

    //calculate movement based on forward and right
    if (!isSprinting)
    {
      Vec3 movement = (forward * inputDirection.z + right * inputDirection.x) * (dt * WalkSpeed);
      transform.SetPosition(pos + movement);
    }
    else
    {
      Vec3 movement = (forward * inputDirection.z + right * inputDirection.x) * (dt * SprintSpeed);
      transform.SetPosition(pos + movement);
    }
  }

  public override void Exit()
  {

  }

  private float DegreesToRadians(float degrees)
  {
    return degrees * ((float)Math.PI) / 180f;
  }
}
