using NITELITE;
using System;

namespace Suika
{
  class Balls : NL_Script
  {
    static bool everyother = true;

    public string BallName = "";

    public readonly string[] BallNames = new string[]
    {
      "BallA", "BallB", "BallC", "BallD", "BallE",
      "BallF", "BallG", "BallH", "BallI", "BallJ", "BallK"
    };

    private Entity[] Dropper;
    private DropperManager DropMana;

    private Entity[] Score;
    private TextComponent ScoreText;

    public override void Init()
    {
      BallName = self.name;
      Dropper = Entity.GetEntitiesByName("Dropper");
      DropMana = Dropper[0].GetComponent<DropperManager>();
      Score = Entity.GetEntitiesByName("Score");
      ScoreText = Score[0].GetComponent<TextComponent>();
    }

    public override void Update()
    {
      
    }

    public override void Exit()
    {
      
    }


    void OnContactAdded(PhysicsComponent me, PhysicsComponent notme, float collisionX, float collisionY, float collisionZ)
    {

      if (me.GetParent().name == notme.GetParent().name)
      {
        if (everyother)
        {
          for (int i = 0; i < BallNames.Length; i++)
          {
            if (BallNames[i] == BallName)
            {
              Entity NewBall = Scene.LoadPrefab($"assets/scenes/Suika/{BallNames[i + 1]}.nlprefab");
              Vec3 SpawnPos = new Vec3(collisionX, collisionY, collisionZ);
              NewBall.GetComponent<Transform>().SetPosition(SpawnPos);

              int CurrentScore = Convert.ToInt32(ScoreText.Text);
              int NewScore = CurrentScore + (i + 2) * 100;
              ScoreText.Text = NewScore.ToString();
            }
          }
        }

        me.GetParent().Destroy();
        everyother = !everyother;
      }
      else
      {
        //NL_INFO(me.GetParent().name + " touched " + notme.GetParent().name);
      }

      //if (a != "null") //Collided ball has Balls script && BallName == Name of the collided ball
      //{
        //NL_INFO(BallName + "s touched");

        //if (everyother)
        //{
        //  for (int i = 0; i < DropMana.BallNames.Length; i++)
        //  {
        //    if (DropMana.BallNames[i] == BallName)
        //    {



        //      return;
        //    }
        //  }
        //}
        

        ////destroy ball
        //everyother = !everyother;
      //}
    }
  }
}
