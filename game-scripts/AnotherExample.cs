
public class AnotherExample : NL_Script
{
  public int i;

  public override void Init()
  {
    NL_INFO("script 2 init");
  }
  public override void Update()
  {

  }

  public override void Exit()
  {
    NL_INFO("script 2 exit");
  }

  public void printi()
  {
    NL_INFO("i: " + i);
  }
}
