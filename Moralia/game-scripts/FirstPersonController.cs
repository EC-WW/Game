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
  public float MoveSpeed = 5f;

  public float RotationSpeed = 3f;

  public override void Init()
  {
        
  }

  public override void Update()
  {
    Rotation();
    Movement();
  }

  private void Rotation()
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

  private void Movement()
    {
    ref Transform transform = ref self.GetComponent<Transform>();

    //create a vec2 for input
    Vec3 inputDirection = new Vec3(0, 0, 0);

    //update the vec2 based on input
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
    Vec3 movement = (forward * inputDirection.z + right * inputDirection.x) * (dt * MoveSpeed);

    //move the entity
    transform.position += movement;
  }

  public override void Exit()
  {

  }
}
