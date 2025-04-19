using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NITELITE;

public struct PipeCollision : Event
{ }


namespace FlappyBird
{
  class Bird : NL_Script
  {

    private bool running = false;

    public override void Init()
    { 
    }

    public override void Update()
    {
      if (!running)
      {
        self.GetComponent<Transform>().SetPosition(new Vec3(0, 2.5f, 20.0f));
      }

      if (Input.GetKeyTriggered(Keys.SPACE))
      {
        running = true;
        self.GetComponent<PhysicsComponent>().Velocity = new Vec3(0, 5, 0);
      }

      if(self.GetComponent<Transform>().GetPosition().y < -2 || self.GetComponent<Transform>().GetPosition().y > 9)
      {
        PipeCollision c = new PipeCollision();
        Events.Signal(ref c);

        self.GetComponent<PhysicsComponent>().Velocity = new Vec3(0, 0, 0);
        self.GetComponent<Transform>().SetPosition(new Vec3(0, 2.5f, 20));
      }
    }

    public override void Exit()
    {

    }

    public void OnContactAdded(PhysicsComponent me, PhysicsComponent other, float collisionX, float collisionY, float collisionZ)
    {
      self.GetComponent<Transform>().SetPosition(new Vec3(0, 2.5f, 20.0f));
      self.GetComponent<PhysicsComponent>().Velocity = new Vec3(0, 0, 0);
      self.GetComponent<PhysicsComponent>().AngularVelocity = new Vec3(0, 0, 0);
      PipeCollision c = new PipeCollision();
      Events.Signal(ref c);
    }
  }
}

