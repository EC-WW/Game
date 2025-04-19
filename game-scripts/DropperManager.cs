using NITELITE;
using System;

namespace Suika
{
  class DropperManager : NL_Script
  {
    private Entity CurrentBall;
    private int NextBallIndex;
    public readonly string[] BallNames = new string[]
    {
      "BallA", "BallB", "BallC", "BallD", "BallE",
      "BallF", "BallG", "BallH", "BallI", "BallJ", "BallK"
    };
    public Entity DropperPivot;
    private Vec3 CurrentPos;
    
    private float CooldownTimer = 0f;
    private float CooldownDuration = 0.777f;
    private bool WaitForBall = false;
    private Entity[] Score;
    private TextComponent ScoreText;

    public override void Init()
    {
      Random r = new Random();
      int RandomInt = r.Next(0, BallNames.Length - 6);
      NextBallIndex = RandomInt;
      Score = Entity.GetEntitiesByName("Score");
      ScoreText = Score[0].GetComponent<TextComponent>();

      SpawnHeldBall();
    }

    public override void Update()
    {
      FollowDropper();

      if (WaitForBall)
      {
        CooldownTimer -= dt;

        if (CooldownTimer < 0f)
        {
          WaitForBall = false;
          SpawnHeldBall();
        }

        return;
      }


      if (NITELITE.Input.GetKeyReleased(Keys.MOUSE_LEFT))
      {
        Drop();
      }
    }

    public void FollowDropper()
    {
      if (!WaitForBall)
      {
        Transform DropperTransform = DropperPivot.GetComponent<Transform>();
        Transform BallTransform = CurrentBall.GetComponent<Transform>();

        Vec3 Offset = new Vec3(0f, 3.5f, 0f);
        BallTransform.SetPosition(DropperTransform.GetPosition() + Offset);

        CurrentPos = DropperTransform.GetPosition() + Offset;
      }
    }

    public void SpawnHeldBall()
    {
      string BallName = BallNames[NextBallIndex];
      string PrefabPath = $"assets/scenes/Suika/{BallName}_Static.nlprefab";

      CurrentBall = Scene.LoadPrefab(PrefabPath);

      Transform BallTransform = CurrentBall.GetComponent<Transform>();
      Vec3 SpawnOffset = new Vec3(0f, 3.5f, 0f);
      BallTransform.SetPosition(DropperPivot.GetComponent<Transform>().GetPosition() + SpawnOffset);
    }

    public void Drop()
    {
      Vec3 DropPos = CurrentBall.GetComponent<Transform>().GetPosition();
      CurrentBall.Destroy();

      string BallName = BallNames[NextBallIndex];
      string PrefabPath = $"assets/scenes/Suika/{BallName}.nlprefab";

      Entity PhysicsBall = Scene.LoadPrefab(PrefabPath);
      Transform BallTransform = PhysicsBall.GetComponent<Transform>();
      BallTransform.SetPosition(CurrentPos);

      int CurrentScore = Convert.ToInt32(ScoreText.Text);
      int NewScore = CurrentScore + (NextBallIndex + 1) * 100;
      ScoreText.Text = NewScore.ToString();

      Random r = new Random();
      int RandomInt = r.Next(0, BallNames.Length - 6);
      NextBallIndex = RandomInt;

      CooldownTimer = CooldownDuration;
      WaitForBall = true;
    }

    public override void Exit()
    {
      
    }
  }
}
