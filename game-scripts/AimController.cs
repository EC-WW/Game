using NITELITE;
using System;

namespace AimTrainer
{
  public struct FPSVariables
  {
    public float Sensitivity;
    public float PitchMin;
    public float PitchMax;
    public float YawMin;
    public float YawMax;
  }

  class AimController : NL_Script
  {
    public FPSVariables Look;

    private float CurrentYaw = 0f;
    private float CurrentPitch = 0f;

    public override void Init()
    {
      Vec3 Rot = self.GetComponent<CameraComponent>().rotation;
      CurrentYaw = Rot.x;
      CurrentPitch = Rot.y;
    }

    public override void Update()
    {
      HandleLook();
      Shoot();
    }
    
    public void HandleLook()
    {
      float mouseX = NITELITE.Input.MouseDelta.x * Look.Sensitivity;
      float mouseY = NITELITE.Input.MouseDelta.y * Look.Sensitivity;

      CurrentYaw += mouseX;
      CurrentPitch -= mouseY;

      CurrentYaw = Math.Clamp(CurrentYaw, Look.YawMin, Look.YawMax);
      CurrentPitch = Math.Clamp(CurrentPitch, Look.PitchMin, Look.PitchMax);

      Vec3 NewRot = new Vec3(CurrentYaw, CurrentPitch, 0f);
      self.GetComponent<CameraComponent>().rotation = NewRot;
    }

    public void Shoot()
    {
      if (NITELITE.Input.GetKeyTriggered(Keys.MOUSE_LEFT))
      {
        Entity Shot = Scene.LoadPrefab($"assets/prefabs/GunPivot.nlprefab");

        Vec3 CamRot = self.GetComponent<CameraComponent>().rotation;
        Vec3 NewRot = new Vec3(DegToRad(CamRot.y), DegToRad(CamRot.x), 0f);
        Shot.GetComponent<Transform>().SetRotation(NewRot);
      }
    }

    public static float DegToRad(float degrees)
    {
      return (float)(degrees * (Math.PI / 180.0f));
    }

    public override void Exit()
    {
      
    }
  }
}
