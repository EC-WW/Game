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

public struct mycoolevent : Event
{
    public int eventdata;
}

public struct hello
{
  public int dog;
  public int cat;
}


public class Example : NL_Script
{
  public int test;
  public hello hi;

  public Entity myent;

  //public float a;
  //public int b;

  private Transform transform;
  private Transform camtransform;

  void mycooleventhandler(ref mycoolevent e)
  {
    Console.WriteLine("C# cool event handler" + e.eventdata);
  }

  void EntityCreatedEvent(ref EntityCreated e)
  {
    Console.WriteLine("C# entity created event handler" + e.entityID);
  }

  void EntityDestroyedEvent(ref EntityDestroyed e)
  {
    Console.WriteLine("C# entity destroyed event handler" + e.entityID);
  }

  void Transformaddevent(ref ComponentAdded<Transform> e)
  {
    Console.WriteLine("C# transform added event handler" + e.entityID);
  }
  public override void Init()
  {
    transform = self.GetComponent<Transform>();
    camtransform = myent.GetComponent<Transform>();

    //Heres your basic debug print, it will get better at some point
    NL_INFO("Hello NITELITE from C#!");

    //NL_INFO(hi.cat.ToString());
    //NL_INFO(hi.dog.ToString());
    NL_INFO(test.ToString());

    //To subscribe to an event, you need a function that takes
    //a reference to the event type you want to subscribe to,
    //and an entity with a script that is subscribing to the event,
    //this is typically the "self" entity

    //These events are built in, the engine will automatically signal them
    //for you when the corresponding event occurs
    Events.Subscribe<EntityCreated>(self, EntityCreatedEvent);
    Events.Subscribe<EntityDestroyed>(self, EntityDestroyedEvent);
    Events.Subscribe<ComponentAdded<Transform>>(self, Transformaddevent);

    //you can also create custom events and signal them whenever you want
    Events.Subscribe<mycoolevent>(self, mycooleventhandler);
  }

  public override void Update()
  {
    //NL_INFO(hi.cat.ToString());
    //NL_INFO(hi.dog.ToString());
    //NL_INFO(test.ToString());

    //Components are accessed like this, if you want to write
    //data into them, you need to use the ref keyword, like so:
    //Because of reasons, these data references are only guranteed to be good
    //for a single frame, so they should generally be locals that you re-get
    //every frame
    //If this changes at some point, I will let you know

    //dt is a variable that you have access to inside of anything that
    //inherits from NL_Script, and has its value refreshed automatically
    Vec3 rot = transform.GetRotation();
    rot.x += 5f * dt;
    rot.y += 5f * dt;
    rot.z -= 5f * dt;
    transform.SetRotation(rot);

    Vec3 camPos = camtransform.GetPosition();
    camPos.x += dt;
    camtransform.SetPosition(camPos);

    if (NITELITE.Input.GetKeyPressed(Keys.W))
    {
      myent.name = "wow";
    }

    if (NITELITE.Input.GetKeyPressed(Keys.A))
    {
      myent.tag = "mowo";
    }

    if (NITELITE.Input.GetKeyPressed(Keys.S))
    {
      NL_INFO(myent.tag);
    }

    if (NITELITE.Input.GetKeyPressed(Keys.D))
    {
      Entity[] entities = Entity.GetEntitiesByName("mowo");
      foreach (Entity e in entities)
      {
        Transform t = e.GetComponent<Transform>();
        NL_INFO(t.GetPosition().x.ToString());
      }
    }

    Vec3 pos = transform.GetPosition();
    //input stuff can be found in NITELITE.Input
    if (NITELITE.Input.GetKeyPressed(Keys.I))
    {
      pos.y += 0.08f;
    }
    if (NITELITE.Input.GetKeyPressed(Keys.J))
    {
      pos.x -= 0.08f;
    }
    if (NITELITE.Input.GetKeyPressed(Keys.K))
    {
      pos.y -= 0.08f;
    }
    if (NITELITE.Input.GetKeyPressed(Keys.L))
    {
      pos.x += 0.08f;
    }

    //heres mouse stuff
    if (NITELITE.Input.GetKeyPressed(Keys.SPACE))
    {
      NL_INFO("Mouse Pos" + NITELITE.Input.MousePosition);
      NL_INFO("Mouse Delta" + NITELITE.Input.MouseDelta);
    }

    if (NITELITE.Input.GetKeyTriggered(Keys.U))
    {
      //you can signal an event like so:
      mycoolevent e;
      e.eventdata = 42;
      Events.Signal(ref e);
    }

    if (NITELITE.Input.GetKeyTriggered(Keys.P))
    {
      //To create an entity, you simply make a new one!
      myent = new Entity();

      //Entities can have components added to them like so,
      //you get back an empty component that you fill out data for
      Vec3 camNewPos = camtransform.GetPosition();
      camNewPos.x = 1.0f;
      camNewPos.y = 2.0f;
      camNewPos.z = 3.0f;
      camNewPos.x = 4.0f;
      camNewPos.y = 5.0f;
      camNewPos.z = 6.0f;
      camtransform.SetPosition(camNewPos);

      Vec3 camScale = camtransform.GetScale();
      camScale.x = 2.0f;
      camScale.y = 2.0f;
      camScale.z = 2.0f;
      camtransform.SetScale(camScale);

      ModelComponent m = myent.AddComponent<ModelComponent>();
      m.modelPath = "trotting_cat";

      //if you want to do something simple like adding a tag
      //you can do it in one line like so:
      myent.tag = "mowo";
    }

    if (NITELITE.Input.GetKeyTriggered(Keys.O))
    {
      //you can destroy an entity like so:
      myent.Destroy();
    }
  }

  public override void Exit()
  {
    NL_INFO("Goodbye NITELITE from C#!");
  }
}
