using Microsoft.VisualBasic;
using NITELITE;
using System;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace Suika
{
  public struct YawVariables
  {
    public Entity YawPivot;
    public float Increment;
  }

  public struct PitchVariables
  {
    public Entity PitchPivot;
    public float Min;
    public float Max;
    public float Increment;
  }

  public struct CameraVariables
  {
    public Entity Main;
    public Entity Pivot;
  }

  public struct DropperVariables
  {
    public Entity DropperPivot;
    public float MoveSpeed;
    public float Boundary;
  }

  class SuikaController : NL_Script
  {
    public YawVariables Yaw;
    public PitchVariables Pitch;
    public CameraVariables Camera;
    public DropperVariables Dropper;

    public override void Init()
    {
      NITELITE.Window.ToggleCursorLock(true);
    }

    public override void Update()
    {
      YawRotation();
      PitchRotation();
      DropperMovement();
      CameraFollow();
      CameraRotation();

      if (NITELITE.Input.GetKeyTriggered(Keys.MOUSE_RIGHT))
      {
        NITELITE.Window.ToggleCursorLock(false);
      }
    }

    public void YawRotation()
    {
      Vec3 CurrentRot = Yaw.YawPivot.GetComponent<Transform>().GetRotation();

      float MouseDeltaX = NITELITE.Input.MouseDelta.x;
      float DeltaYaw = MouseDeltaX * Yaw.Increment;
      float NewY = CurrentRot.y + DeltaYaw;

      Vec3 NewRot = new Vec3(CurrentRot.x, NewY, CurrentRot.z);
      Yaw.YawPivot.GetComponent<Transform>().SetRotation(NewRot);
    }

    public void PitchRotation()
    {
      Vec3 CurrentRot = Pitch.PitchPivot.GetComponent<Transform>().GetRotation();

      float MouseDeltaY = NITELITE.Input.MouseDelta.y;
      float DeltaPitch = MouseDeltaY * Pitch.Increment;
      float NewX = CurrentRot.x - DeltaPitch;

      NewX = Math.Clamp(NewX, Pitch.Min, Pitch.Max);
      Vec3 NewRot = new Vec3(NewX, CurrentRot.y, CurrentRot.z);
      Pitch.PitchPivot.GetComponent<Transform>().SetRotation(NewRot);
    }

    public void DropperMovement()
    {
      Vec3 InputDir = new Vec3();

      if (NITELITE.Input.GetKeyPressed(Keys.W))
      {
        InputDir.z -= 1;
      }

      if (NITELITE.Input.GetKeyPressed(Keys.A))
      {
        InputDir.x += 1;
      }

      if (NITELITE.Input.GetKeyPressed(Keys.S))
      {
        InputDir.z += 1;
      }

      if (NITELITE.Input.GetKeyPressed(Keys.D))
      {
        InputDir.x -= 1;
      }

      if (InputDir.Magnitude() > 0.0001f)
      {
        InputDir.Normalize();
      }

      float YawDeg = Yaw.YawPivot.GetComponent<Transform>().GetRotation().y;
      Vec3 Forward = new Vec3(MathF.Sin(YawDeg), 0, MathF.Cos(YawDeg));
      Vec3 Right = new Vec3(Forward.z, 0, -Forward.x);

      Vec3 MoveDir = Forward * InputDir.z + Right * InputDir.x;
      MoveDir *= Dropper.MoveSpeed * dt;

      Transform DropperTransform = Dropper.DropperPivot.GetComponent<Transform>();
      Vec3 CurrentPos = DropperTransform.GetPosition();
      Vec3 NewPos = CurrentPos + MoveDir;

      NewPos.x = Math.Clamp(NewPos.x, -Dropper.Boundary, Dropper.Boundary);
      NewPos.z = Math.Clamp(NewPos.z, -Dropper.Boundary, Dropper.Boundary);

      DropperTransform.SetPosition(NewPos);
    }

    private void CameraFollow()
    {
      Transform yawTransform = Yaw.YawPivot.GetComponent<Transform>();
      Transform pitchTransform = Pitch.PitchPivot.GetComponent<Transform>();
      Transform camPivotTransform = Camera.Pivot.GetComponent<Transform>();

      // Get local positions
      Vec3 yawPos = yawTransform.GetPosition();
      Vec3 pitchLocalPos = pitchTransform.GetPosition();
      Vec3 camLocalPos = camPivotTransform.GetPosition();

      // Get local rotations in degrees
      float yawDeg = yawTransform.GetRotation().y;
      float pitchDeg = pitchTransform.GetRotation().x;

      // --- Rotate camLocalPos by pitch (around X-axis) ---
      float px = camLocalPos.x;
      float py = camLocalPos.y * MathF.Cos(pitchDeg) - camLocalPos.z * MathF.Sin(pitchDeg);
      float pz = camLocalPos.y * MathF.Sin(pitchDeg) + camLocalPos.z * MathF.Cos(pitchDeg);
      Vec3 pitchRotated = new Vec3(px, py, pz);

      // --- Rotate pitchPivot position by pitch too ---
      float ppx = pitchLocalPos.x;
      float ppy = pitchLocalPos.y * MathF.Cos(pitchDeg) - pitchLocalPos.z * MathF.Sin(pitchDeg);
      float ppz = pitchLocalPos.y * MathF.Sin(pitchDeg) + pitchLocalPos.z * MathF.Cos(pitchDeg);
      Vec3 pitchOffsetRotated = new Vec3(ppx, ppy, ppz);

      Vec3 localFromYaw = pitchRotated + pitchOffsetRotated;

      // --- Rotate by yaw (around Y-axis) ---
      float fx = localFromYaw.x * MathF.Cos(yawDeg) - localFromYaw.z * MathF.Sin(yawDeg);
      float fy = localFromYaw.y;
      float fz = localFromYaw.x * MathF.Sin(yawDeg) + localFromYaw.z * MathF.Cos(yawDeg);
      Vec3 finalOffset = new Vec3(fx, fy, fz);

      // Final world position
      Vec3 worldPos = yawPos + finalOffset;
      worldPos = new Vec3(-worldPos.x, worldPos.y, worldPos.z);

      // Set the camera’s position
      Camera.Main.GetComponent<Transform>().SetPosition(worldPos);
    }

    public void CameraRotation()
    {
      Vec3 camPos = Camera.Main.GetComponent<Transform>().GetPosition();
      Vec3 targetPos = Yaw.YawPivot.GetComponent<Transform>().GetPosition();
      Vec3 lookDir = (targetPos - camPos);
      lookDir.Normalize();

      // Manually invert the direction if needed
      Vec3 flippedDir = new Vec3(lookDir.x, lookDir.y, lookDir.z);

      Camera.Main.GetComponent<CameraComponent>().facingDirection = flippedDir;
    }

    public override void Exit()
    {

    }
  }
}