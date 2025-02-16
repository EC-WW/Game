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

public class Example : NL_Script
{

  Entity myent;

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
    //Heres your basic debug print, it will get better at some point
    NL_INFO("Hello NITELITE from C#!");

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
    
    //Components are accessed like this, if you want to write
    //data into them, you need to use the ref keyword, like so:
    ref Transform transform = ref self.GetComponent<Transform>();
    //Because of reasons, these data references are only guranteed to be good
    //for a single frame, so they should generally be locals that you re-get
    //every frame
    //If this changes at some point, I will let you know

    //dt is a variable that you have access to inside of anything that
    //inherits from NL_Script, and has its value refreshed automatically
    transform.rotation.x += 5f * dt;
    transform.rotation.y += 5f * dt;
    transform.rotation.z -= 5f * dt;

    //input stuff can be found in NITELITE.Input
    if (NITELITE.Input.GetKeyPressed(Keys.I))
    {
      transform.position.y += 0.08f;
    }
    if (NITELITE.Input.GetKeyPressed(Keys.J))
    {
      transform.position.x -= 0.08f;
    }
    if (NITELITE.Input.GetKeyPressed(Keys.K))
    {
      transform.position.y -= 0.08f;
    }
    if (NITELITE.Input.GetKeyPressed(Keys.L))
    {
      transform.position.x += 0.08f;
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
      ref Transform t = ref myent.AddComponent<Transform>();
      t.position.x = 1.0f;
      t.position.y = 2.0f;
      t.position.z = 3.0f;
      t.rotation.x = 4.0f;
      t.rotation.y = 5.0f;
      t.rotation.z = 6.0f;

      t.scale.x = 2.0f;
      t.scale.y = 2.0f;
      t.scale.z = 2.0f;

      ref ModelComponent m = ref myent.AddComponent<ModelComponent>();
      m.modelPath = "trotting_cat";

      //if you want to do something simple like adding a tag
      //you can do it in one line like so:
      myent.AddComponent<TagComponent>().tag = "mowo";
    }

    if (NITELITE.Input.GetKeyTriggered(Keys.O))
    {
      //you can destroy an entity like so:
      myent.Destroy();

      //entities are only actually destroyed at the end of the frame,
      //so you can still access their data until then
      ref Transform t = ref myent.GetComponent<Transform>();
      NL_INFO(t.position.x.ToString());
      NL_INFO(t.position.y.ToString());
      NL_INFO(t.position.z.ToString());

      ref TagComponent tag = ref myent.GetComponent<TagComponent>();
      NL_INFO(tag.tag);
    }
  }

  public override void Exit()
  {
    NL_INFO("Goodbye NITELITE from C#!");
  }
}
