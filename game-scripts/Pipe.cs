using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NITELITE;

namespace FlappyBird
{
  class PipeSegment : NL_Script
  {
    //public Transform Top1;
    //public Transform Top2;

    public override void Init()
    {
    }

    public override void Update()
    {
      self.GetComponent<Transform>().SetRotation(new Vec3(0, 0, 0));
      //Top1.SetRotation(new Vec3(0, 0, 0));
      //Top2.SetRotation(new Vec3(0, 0, 0));
    }

    public override void Exit()
    {
    }
  }
}
