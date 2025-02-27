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

  public override void Init()
  {
    ref Transform transform = ref Camera.GetComponent<Transform>();
    transform.rotation.x = 180f;
  }

  public override void Update()
  {
    //DebugRotation();
    CameraRotation();
    CameraFollow();

    PlayerInput();
    Movement();
  }

  private void DebugRotation()
  {
    ref Transform transform = ref self.GetComponent<Transform>();

    if (NITELITE.Input.GetKeyPressed(Keys.Q))
    {
      transform.rotation.y -= dt * RotationSpeed;
    }
    if (NITELITE.Input.GetKeyPressed(Keys.E))
    {
      transform.rotation.y += dt * RotationSpeed;
    }
  }

  private void CameraRotation()
  {
    ref Transform transform = ref self.GetComponent<Transform>();
    transform.rotation.y = RadiansToDegrees(Camera.GetComponent<CameraComponent>().rotation.x) - RadiansToDegrees(Camera.GetComponent<Transform>().rotation.x);
  }

  private void CameraFollow()
  {
    ref Transform transform = ref self.GetComponent<Transform>();
    Camera.GetComponent<Transform>().position = transform.position + CamOffset;
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
    ref Transform transform = ref self.GetComponent<Transform>();
    
    //normalize input direction to prevent diagonal speed boost
    if (inputDirection.x != 0 || inputDirection.z != 0)
    {
      inputDirection.Normalize();
    }

    //get the forward and right vectors based on rotation
    float radians = transform.rotation.y;
    Vec3 forward = new Vec3(MathF.Sin(radians), 0, MathF.Cos(radians));
    Vec3 right = new Vec3(forward.z, 0, -forward.x);

    //calculate movement based on forward and right
    if (!isSprinting)
    {
      Vec3 movement = (forward * inputDirection.z + right * inputDirection.x) * (dt * WalkSpeed);
      transform.position += movement;
    }
    else
    {
      Vec3 movement = (forward * inputDirection.z + right * inputDirection.x) * (dt * SprintSpeed);
      transform.position += movement;
    }
  }

  public override void Exit()
  {

  }

  private float RadiansToDegrees(float radians)
  {
    return radians * ((float)Math.PI) / 180f;
  }
}
