/**
/ file:   FirstPersonController_NEW.cs
/ author: taylor.cadwallader
/ date:   April 16, 2024
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  First person player controller for a 3d environment.
**/
using System;
using System.Numerics;
using NITELITE;

public class FirstPersonController_NEW : NL_Script
{
  public float MoveSpeed = 5f;

  public Entity camEnt;
  private CameraComponent camera;
  private Transform camTransform;
  private Transform transform;
  private Vec3 newRotation;

  public override void Init()
  {
    camera = camEnt.GetComponent<CameraComponent>();
    camTransform = camEnt.GetComponent<Transform>();

    transform = self.GetComponent<Transform>();
  }

  public override void Update()
  {
    RotateWithCam();
    Movement();
  }


  private void RotateWithCam()
  {
    //set the new rotation to the camera's
    newRotation.y = camera.rotation.x * ((float)Math.PI) / 180f - camTransform.GetRotation().x * ((float)Math.PI) / 180f;

    //finally set the rotation of the player entity to match camera rotation
    transform.SetRotation(newRotation);
  }

  private void Movement()
  {
    //create a new vec for input
    Vec3 inputDirection = Vec3.Zero;
    //update the vec accordingly
    if (NITELITE.Input.GetKeyPressed(Keys.W))
      inputDirection.z -= 1;
    if (NITELITE.Input.GetKeyPressed(Keys.A))
      inputDirection.x += 1;
    if (NITELITE.Input.GetKeyPressed(Keys.S))
      inputDirection.z += 1;
    if (NITELITE.Input.GetKeyPressed(Keys.D))
      inputDirection.x -= 1;

    //normalize input direction to prevent diagonal speed boost
    if (inputDirection.x != 0 || inputDirection.z != 0)
      inputDirection.Normalize();

    //get the forward and right vectors based on rotation
    float radians = transform.GetRotation().y;
    Vec3 forward = new Vec3(MathF.Sin(radians), 0, MathF.Cos(radians));
    Vec3 right = new Vec3(forward.z, 0, -forward.x);

    //calculate movement based on forward and right
    Vec3 movement = (forward * inputDirection.z + right * inputDirection.x) * (dt * MoveSpeed);

    //move the entity
    Vec3 currentPos = transform.GetPosition();
    transform.SetPosition(currentPos += movement);
  }

  public override void Exit()
  {

  }
}
