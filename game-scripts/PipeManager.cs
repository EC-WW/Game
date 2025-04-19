using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NITELITE;

namespace FlappyBird
{

  public class PipeManager : NL_Script
  {
    public Transform Pipe1;
    public Transform Pipe2;
    public Transform Pipe3;
    public Transform Pipe4;

    public TextComponent ScoreText;

    int score = 0;

    public float StartSpeed;
    public float Acceleration;

    private float speed;
    private Random random = new Random();

    private bool running = false;

    public void OnPipeCollision(ref PipeCollision c)
    {
      Reset();
    }


    private float randomYPos()
    {
     int randint = random.Next(0, 100);
      return ((float)randint)/100;
    }

    public override void Init()
    {
      speed = StartSpeed;
      Events.Subscribe<PipeCollision>(self, OnPipeCollision);
    }
    
    public void Reset()
    {
      speed = StartSpeed;

      Pipe1.SetPosition(new Vec3(0, randomYPos(), 20));
      Pipe2.SetPosition(new Vec3(8, randomYPos(), 20));
      Pipe3.SetPosition(new Vec3(0, randomYPos(), 20));
      Pipe4.SetPosition(new Vec3(8, randomYPos(), 20));
      score = 0;
      ScoreText.Text = score.ToString();
    }

    void UpdatePipe(Transform t)
    {
      Vec3 pos = t.GetPosition();
      if (pos.x < -8)
      {
        pos.x = 8;
        pos.y = randomYPos();
        score++;
        ScoreText.Text = (score/2).ToString();
      }
      else
      {
        pos.x -= speed * dt;
      }
      t.SetPosition(pos);
    }

    public override void Update()
    {

      if (Input.GetKeyTriggered(Keys.SPACE))
      {
        running = true;
      }

      if (!running)
        return;

      speed += Acceleration * dt;
      UpdatePipe(Pipe1);
      UpdatePipe(Pipe2);
      UpdatePipe(Pipe3);
      UpdatePipe(Pipe4);
    }

    public override void Exit()
    {
    }
  }
}
