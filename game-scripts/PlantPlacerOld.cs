/**
/ file:   PlantPlacer.cs
/ author: aditya.prakash
/ date:   April 16, 2025
/ Copyright (c) 2025 DigiPen (USA) Corporation. 
/ 
/ brief:  Raycast from screen click to find a "Cube" and log its position,
/         with all math done manually (no System.Numerics).
**/
/*
using System;
using NITELITE;

public static class Math3
{
  // free‑standing cross product
  public static Vec3 Cross(Vec3 a, Vec3 b)
  {
    return new Vec3(
      a.y * b.z - a.z * b.y,
      a.z * b.x - a.x * b.z,
      a.x * b.y - a.y * b.x
    );
  }

  // free‑standing dot product
  public static float Dot(Vec3 a, Vec3 b)
  {
    return a.x * b.x + a.y * b.y + a.z * b.z;
  }
}

public struct Mat4
{
  // row-major
  public float m00, m01, m02, m03;
  public float m10, m11, m12, m13;
  public float m20, m21, m22, m23;
  public float m30, m31, m32, m33;

  public static Mat4 Identity => new Mat4
  {
    m00 = 1,
    m11 = 1,
    m22 = 1,
    m33 = 1
  };

  public static Mat4 CreateLookAt(Vec3 eye, Vec3 target, Vec3 up)
  {
    Vec3 f = target - eye; f.Normalize();
    Vec3 s = Math3.Cross(up, f); s.Normalize();
    Vec3 u = Math3.Cross(f, s);

    Mat4 M = new Mat4();
    M.m00 = s.x; M.m01 = u.x; M.m02 = f.x; M.m03 = 0;
    M.m10 = s.y; M.m11 = u.y; M.m12 = f.y; M.m13 = 0;
    M.m20 = s.z; M.m21 = u.z; M.m22 = f.z; M.m23 = 0;
    M.m30 = -Math3.Dot(s, eye);
    M.m31 = -Math3.Dot(u, eye);
    M.m32 = -Math3.Dot(f, eye);
    M.m33 = 1;
    return M;
  }

  public static Mat4 CreatePerspectiveFieldOfView(float fovY, float aspect, float znear, float zfar)
  {
    float yScale = 1f / (float)Math.Tan(fovY * 0.5f);
    float xScale = yScale / aspect;

    Mat4 M = new Mat4();
    M.m00 = xScale; M.m11 = yScale;
    M.m22 = zfar / (znear - zfar);
    M.m23 = -1;
    M.m32 = (znear * zfar) / (znear - zfar);
    return M;
  }

  public static bool Invert(Mat4 src, out Mat4 dst)
  {
    // (same inversion code as you provided...)
    float[] m = {
      src.m00, src.m01, src.m02, src.m03,
      src.m10, src.m11, src.m12, src.m13,
      src.m20, src.m21, src.m22, src.m23,
      src.m30, src.m31, src.m32, src.m33
    };
    float[] inv = new float[16];

    inv[0] = m[5] * m[10] * m[15] - m[5] * m[11] * m[14] - m[9] * m[6] * m[15]
              + m[9] * m[7] * m[14] + m[13] * m[6] * m[11] - m[13] * m[7] * m[10];
    inv[4] = -m[4] * m[10] * m[15] + m[4] * m[11] * m[14] + m[8] * m[6] * m[15]
              - m[8] * m[7] * m[14] - m[12] * m[6] * m[11] + m[12] * m[7] * m[10];
    inv[8] = m[4] * m[9] * m[15] - m[4] * m[11] * m[13] - m[8] * m[5] * m[15]
              + m[8] * m[7] * m[13] + m[12] * m[5] * m[11] - m[12] * m[7] * m[9];
    inv[12] = -m[4] * m[9] * m[14] + m[4] * m[10] * m[13] + m[8] * m[5] * m[14]
              - m[8] * m[6] * m[13] - m[12] * m[5] * m[10] + m[12] * m[6] * m[9];

    inv[1] = -m[1] * m[10] * m[15] + m[1] * m[11] * m[14] + m[9] * m[2] * m[15]
              - m[9] * m[3] * m[14] - m[13] * m[2] * m[11] + m[13] * m[3] * m[10];
    inv[5] = m[0] * m[10] * m[15] - m[0] * m[11] * m[14] - m[8] * m[2] * m[15]
              + m[8] * m[3] * m[14] + m[12] * m[2] * m[11] - m[12] * m[3] * m[10];
    inv[9] = -m[0] * m[9] * m[15] + m[0] * m[11] * m[13] + m[8] * m[1] * m[15]
              - m[8] * m[3] * m[13] - m[12] * m[1] * m[11] + m[12] * m[3] * m[9];
    inv[13] = m[0] * m[9] * m[14] - m[0] * m[10] * m[13] - m[8] * m[1] * m[14]
              + m[8] * m[2] * m[13] + m[12] * m[1] * m[10] - m[12] * m[2] * m[9];

    inv[2] = m[1] * m[6] * m[15] - m[1] * m[7] * m[14] - m[5] * m[2] * m[15]
              + m[5] * m[3] * m[14] + m[13] * m[2] * m[7] - m[13] * m[3] * m[6];
    inv[6] = -m[0] * m[6] * m[15] + m[0] * m[7] * m[14] + m[4] * m[2] * m[15]
              - m[4] * m[3] * m[14] - m[12] * m[2] * m[7] + m[12] * m[3] * m[6];
    inv[10] = m[0] * m[5] * m[15] - m[0] * m[7] * m[13] - m[4] * m[1] * m[15]
              + m[4] * m[3] * m[13] + m[12] * m[1] * m[7] - m[12] * m[3] * m[5];
    inv[14] = -m[0] * m[5] * m[14] + m[0] * m[6] * m[13] + m[4] * m[1] * m[14]
              - m[4] * m[2] * m[13] - m[12] * m[1] * m[6] + m[12] * m[2] * m[5];

    inv[3] = -m[1] * m[6] * m[11] + m[1] * m[7] * m[10] + m[5] * m[2] * m[11]
              - m[5] * m[3] * m[10] - m[9] * m[2] * m[7] + m[9] * m[3] * m[6];
    inv[7] = m[0] * m[6] * m[11] - m[0] * m[7] * m[10] - m[4] * m[2] * m[11]
              + m[4] * m[3] * m[10] + m[8] * m[2] * m[7] - m[8] * m[3] * m[6];
    inv[11] = -m[0] * m[5] * m[11] + m[0] * m[7] * m[9] + m[4] * m[1] * m[11]
              - m[4] * m[3] * m[9] - m[8] * m[1] * m[7] + m[8] * m[3] * m[5];
    inv[15] = m[0] * m[5] * m[10] - m[0] * m[6] * m[9] - m[4] * m[1] * m[10]
              + m[4] * m[2] * m[9] + m[8] * m[1] * m[6] - m[8] * m[2] * m[5];

    float det = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];
    if (Math.Abs(det) < 1e-6f) { dst = Identity; return false; }
    det = 1f / det;
    for (int i = 0; i < 16; i++) inv[i] *= det;

    dst = new Mat4
    {
      m00 = inv[0],
      m01 = inv[1],
      m02 = inv[2],
      m03 = inv[3],
      m10 = inv[4],
      m11 = inv[5],
      m12 = inv[6],
      m13 = inv[7],
      m20 = inv[8],
      m21 = inv[9],
      m22 = inv[10],
      m23 = inv[11],
      m30 = inv[12],
      m31 = inv[13],
      m32 = inv[14],
      m33 = inv[15]
    };
    return true;
  }

  public static Vec4 Transform(Mat4 m, Vec4 v)
  {
    return new Vec4(
      m.m00 * v.x + m.m01 * v.y + m.m02 * v.z + m.m03 * v.w,
      m.m10 * v.x + m.m11 * v.y + m.m12 * v.z + m.m13 * v.w,
      m.m20 * v.x + m.m21 * v.y + m.m22 * v.z + m.m23 * v.w,
      m.m30 * v.x + m.m31 * v.y + m.m32 * v.z + m.m33 * v.w
    );
  }
}

public struct Ray
{
  public Vec3 origin;
  public Vec3 direction;
}

public class PlantPlacerOld : NL_Script
{
  public Entity Camera;
  private CameraComponent camProps;
  private Transform camTrans;
  private float screenWidth, screenHeight;

  public override void Init()
  {
    camProps = Camera.GetComponent<CameraComponent>();
    camTrans = Camera.GetComponent<Transform>();
    screenWidth = 1280;  // or fetch from your engine
    screenHeight = 720;
  }

  public override void Update()
  {
    if (Input.GetKeyTriggered(Keys.MOUSE_LEFT))
    {
      NL_INFO("Mouse clicked");
      Vec2 mpos = Input.MousePosition;
      // log mouse pos
      NL_INFO($"Mouse pos: {mpos.x}, {mpos.y}");
      Ray ray = ScreenPointToRay(mpos);

      Entity[] cubes = Entity.GetEntitiesByName("Cube");
      float closestT = float.MaxValue;
      int hitIndex = 0;
      bool found = false;

      for (int i = 0; i < cubes.Length; ++i)
      {
        var cube = cubes[i];

        var tf = cube.GetComponent<Transform>();
        Vec3 half = new Vec3(0.5f, 0.5f, 0.5f);
        Vec3 min = tf.GetPosition() - half;
        Vec3 max = tf.GetPosition() + half;

        if (RayAABBIntersect(ray, min, max, out float t) && t < closestT)
        {
          closestT = t;
          hitIndex = i;
          found = true;
        }
      }

      if (found)
      {
        Entity hitCube = cubes[hitIndex];
        Vec3 p = hitCube.GetComponent<Transform>().GetPosition();
        NL_INFO("Clicked " + hitCube.name + " at " + p.x + " " + p.y + " " + p.z);
      }
    }
  }

  public override void Exit() { }

  private Ray ScreenPointToRay(Vec2 sp)
  {
    float nx = (2f * sp.x / screenWidth) - 1f;
    float ny = 1f - (2f * sp.y / screenHeight);

    //NL_INFO($"Click NDC: {nx}, {ny}");

    Vec3 eye = camTrans.GetPosition();
    Vec3 target = eye + camProps.facingDirection;
    Vec3 up = camProps.up;

    Mat4 view = Mat4.CreateLookAt(eye, target, up);
    Mat4 proj = Mat4.CreatePerspectiveFieldOfView(
      50,
      screenWidth / screenHeight,
      camProps.nearClippingPlane,
      camProps.farClippingPlane
    );

    Mat4.Invert(proj, out var invProj);
    Mat4.Invert(view, out var invView);

    Vec4 clip = new Vec4(nx, ny, -1f, 1f);
    Vec4 eyeV = Mat4.Transform(invProj, clip);
    eyeV.z = -1f; eyeV.w = 0f;
    Vec4 world4 = Mat4.Transform(invView, eyeV);

    Vec3 dir = new Vec3(world4.x, world4.y, world4.z);
    dir.Normalize();

    // log eye and dir
    //NL_INFO($"Eye: {eye.x}, {eye.y}, {eye.z}");
    //NL_INFO($"Dir: {dir.x}, {dir.y}, {dir.z}");

    Entity newEnt = Entity.Create();
    newEnt.AddComponent<Transform>();
    newEnt.GetComponent<Transform>().SetPosition(eye - dir);
    //newEnt.GetComponent<Transform>().SetScale(new Vec3(0.1f, 0.1f, 0.1f));
    newEnt.AddComponent<ModelComponent>();
    newEnt.GetComponent<ModelComponent>().modelPath = "Cube";

    return new Ray { origin = eye, direction = dir };
  }

  private bool RayAABBIntersect(Ray r, Vec3 bmin, Vec3 bmax, out float t)
  {
    float tmin = (bmin.x - r.origin.x) / r.direction.x;
    float tmax = (bmax.x - r.origin.x) / r.direction.x;
    if (tmin > tmax) Swap(ref tmin, ref tmax);

    float tymin = (bmin.y - r.origin.y) / r.direction.y;
    float tymax = (bmax.y - r.origin.y) / r.direction.y;
    if (tymin > tymax) Swap(ref tymin, ref tymax);

    if (tmin > tymax || tymin > tmax) { t = 0; return false; }
    if (tymin > tmin) tmin = tymin;
    if (tymax < tmax) tmax = tymax;

    float tzmin = (bmin.z - r.origin.z) / r.direction.z;
    float tzmax = (bmax.z - r.origin.z) / r.direction.z;
    if (tzmin > tzmax) Swap(ref tzmin, ref tzmax);

    if (tmin > tzmax || tzmin > tmax) { t = 0; return false; }
    t = Math.Max(tmin, tzmin);
    return true;
  }

  private void Swap(ref float a, ref float b)
  {
    float tmp = a; a = b; b = tmp;
  }
}
*/