/**
/ file:   FirstPersonController_NEW.cs
/ author: taylor.cadwallader
/ date:   April 16, 2025
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  First person player controller for a 3d environment.
**/
using System;
using System.Numerics;
using NITELITE;

public class FirstPersonController_NEW : NL_Script
{
  public bool usePhysics = true;
  public float MoveSpeed = 5f;
  public float JumpForce = 10f;
  public float Gravity = 1.5f;
  public Entity camEnt;

  private Transform transform;
  private CameraComponent camera;
  private Transform camTransform;
  private PhysicsComponent rigidbody;

  public bool StopEverything = false;

  private bool mouseLocked = true;

  public bool ConfineMovement = false;
  public Vec3 maxMovePos;
  public Vec3 minMovePos;

  public bool FlipDirectionsForSomeReason = false;

  public override void Init()
  {
    transform = self.GetComponent<Transform>();
    camera = camEnt.GetComponent<CameraComponent>();
    camTransform = camEnt.GetComponent<Transform>();
    rigidbody = self.GetComponent<PhysicsComponent>();
    rigidbody.GravityFactor = Gravity;

    //omg lock the cursor
    NITELITE.Window.ToggleCursorLock(true);
  }

  public override void Update()
  {
    if (StopEverything) return;

    RotateWithCam();
    HandleMovement();
    HandleJumping();

    StupidMovement();


    //DEBUGGING
    if (NITELITE.Input.GetKeyTriggered(Keys.MOUSE_RIGHT))
    {
      mouseLocked = !mouseLocked;
      NITELITE.Window.ToggleCursorLock(mouseLocked);
    }
  }


  private void RotateWithCam()
  {
    Vec3 newRotation = Vec3.Zero;
    //set the new rotation to the camera's
    newRotation.y = camera.rotation.x * ((float)Math.PI) / 180f - camTransform.GetRotation().x * ((float)Math.PI) / 180f;

    //finally set the rotation of the player entity to match camera rotation
    transform.SetRotation(newRotation);
  }

  private void HandleMovement()
  {
    if (!usePhysics) return;

    //create a new vec for input
    Vec3 inputDirection = Vec3.Zero;
    //update the vec accordingly
    if (NITELITE.Input.GetKeyPressed(Keys.W))
      inputDirection.z += -1;
    if (NITELITE.Input.GetKeyPressed(Keys.A))
      inputDirection.x += 1;
    if (NITELITE.Input.GetKeyPressed(Keys.S))
      inputDirection.z += 1;
    if (NITELITE.Input.GetKeyPressed(Keys.D))
      inputDirection.x += -1;

    //normalize input direction to prevent diagonal speed boost
    if (inputDirection.x != 0 || inputDirection.z != 0)
      inputDirection.Normalize();

    //get the forward and right vectors based on rotation
    float radians = transform.GetRotation().y;
    Vec3 forward = new Vec3(MathF.Sin(radians), 0, MathF.Cos(radians));
    Vec3 right = new Vec3(forward.z, 0, -forward.x);

    //calculate movement based on forward and right
    Vec3 movement = (forward * inputDirection.z + right * inputDirection.x);

    Vec3 vel = rigidbody.Velocity;
    vel.x = movement.x * MoveSpeed;
    vel.z = movement.z * MoveSpeed;
    rigidbody.Velocity = vel;
  }

  private void HandleJumping()
  {
    if (!usePhysics) return;

    if (NITELITE.Input.GetKeyTriggered(Keys.SPACE))
    {
      Vec3 vel = rigidbody.Velocity;
      vel.y = JumpForce;
      rigidbody.Velocity = vel;
    }
  }

  private void StupidMovement()
  {
    if (usePhysics) return;

    Vec3 currentPos = transform.GetPosition();

    //create a new vec for input
    Vec3 inputDirection = Vec3.Zero;

    //update the vec accordingly
    if (NITELITE.Input.GetKeyPressed(Keys.W))
      inputDirection.z += -1;
    if (NITELITE.Input.GetKeyPressed(Keys.A))
      inputDirection.x += 1;
    if (NITELITE.Input.GetKeyPressed(Keys.S))
      inputDirection.z += 1;
    if (NITELITE.Input.GetKeyPressed(Keys.D))
      inputDirection.x += -1;

    //normalize input direction to prevent diagonal speed boost
    if (inputDirection.x != 0 || inputDirection.z != 0)
      inputDirection.Normalize();

    //get the forward and right vectors based on rotation
    float radians = transform.GetRotation().y;
    Vec3 forward = new Vec3(MathF.Sin(radians), 0, MathF.Cos(radians));
    Vec3 right = new Vec3(forward.z, 0, -forward.x);

    Vec3 movement = new Vec3();
    if (!FlipDirectionsForSomeReason)
    {
      //calculate movement based on forward and right
      movement = (forward * inputDirection.z + right * inputDirection.x) * (dt * MoveSpeed);
    }
    else
    {
      //calculate movement based on forward and right
      movement = (forward * -inputDirection.z + right * -inputDirection.x) * (dt * MoveSpeed);
    }

    //please for the love of god
    if (ConfineMovement)
    {
      Vec3 predictedPos = currentPos + movement;

      // Clamp the predicted position to the allowed bounds
      predictedPos.x = MathF.Min(MathF.Max(predictedPos.x, minMovePos.x), maxMovePos.x);
      predictedPos.z = MathF.Min(MathF.Max(predictedPos.z, minMovePos.z), maxMovePos.z);

      // Now recalculate the final movement based on clamped position
      movement = predictedPos - currentPos;
    }

    //move the entity
    transform.SetPosition(currentPos += movement);
  }

  public override void Exit()
  {

  }
}
