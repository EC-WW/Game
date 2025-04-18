/**
/ file:   Example.cs
/ author: j.m.love
/ date:   November 14, 2024
/ Copyright (c) 2024 DigiPen (USA) Corporation. 
/ 
/ brief:  Example script for the NITELITE engine, uses several of its features
**/
using System;
using NITELITE;

public struct test2
{
  public float f2;
  public Entity e;
  public string s2;
  public test t;
}

public struct test
{
  public float f;
  public string s;
  public int i;
}

public class testclass
{
  //public float f;
  //public string s;
  //public int i;
  //public void print()
  //{
  //  Console.WriteLine(s);
  //}
}



public class Example : NL_Script
{

  public Entity ent;
  //public Entity Camera;

  public int a;

  public ParticleEmitter particleEmitter;

  public AnotherExample ae;

  private PhysicsComponent p;
  private Transform CamTrans;
  private Transform EntTrans;
    
  //called on creation
  public override void Init()
  {
    NL_INFO("Hello NITELITE from C#!");
    NL_INFO(a.ToString());
    //CamTrans = Camera.GetComponent<Transform>();
    EntTrans = ent.GetComponent<Transform>();

    p = ent.GetComponent<PhysicsComponent>();
  }

  //called every frame
  public override void Update()
  {
    //NL_INFO("1");
    Vec3 camPos = EntTrans.GetPosition();
    camPos = ent.GetComponent<Transform>().GetPosition();
    //NL_INFO("2");

    camPos.z -= 5.0f;
    CamTrans.SetPosition(camPos); 
    //NL_INFO("3");


    if (NITELITE.Input.GetKeyPressed(Keys.P))
    {
      NL_INFO("P pressed");
      // PhysicsComponent p = ent.GetComponent<PhysicsComponent>();
      //p.AddForce(new Vec3(0.0f, 10000.0f, 0));

      Vec3 vel = p.Velocity;
      vel.y = 10.0f;
      p.Velocity = vel;


      Vec3 RotVel = p.AngularVelocity;
      RotVel.y = 100;
      RotVel.x = 70;
      p.AngularVelocity = RotVel;
      //p.SetLinearVelocity(new Vec3(0, 10.0f, 0));
      //p.SetAngularVelocity(new Vec3(5.0f, 5.0f, 0.0f));
    }

    if (NITELITE.Input.GetKeyPressed(Keys.O))
    {
      //PhysicsComponent p = ent.GetComponent<PhysicsComponent>();
      Vec3 vel = p.Velocity;
      NL_INFO(vel.ToString());
    }

    //omg is this c#
    if (NITELITE.Input.GetKeyPressed(Keys.L))
    {
      particleEmitter.duration = 2.0f;
      particleEmitter.emissionRate = 100.0f;
      particleEmitter.velocity = new Vec3(0.0f, 10.0f, 0.0f);
      particleEmitter.velocityVariation = new Vec3(5.0f, 10.0f, 5.0f);
      particleEmitter.acceleration = new Vec3(0.0f, -10.0f, 0.0f);
      particleEmitter.accelerationVariation = new Vec3(0.0f, 4.0f, 0.0f);
      particleEmitter.angularVelocity = new Vec3(0.0f, 0.0f, 0.0f);
      particleEmitter.angularVelocityVariation = new Vec3(1.0f, 0.0f, 0.0f);

      particleEmitter.sizeBegin = 0.25f;
      particleEmitter.sizeEnd = 0.5f;
      particleEmitter.sizeVariation = 0.5f;

      particleEmitter.lifeTime = 2.0f;

      particleEmitter.tint = new Vec4(0.23f, 0.89f, 0.95f, 1.0f);
      particleEmitter.fadeStyle = ParticleEmitter.PARTICLE_FADE_STYLE.FADE_IN;

      particleEmitter.ModelPath = "Cat Plushy";
    }

  }

  //called on exit
  public override void Exit()
  {
    NL_INFO("GoodBye NITELITE From C#");
  }
}
