// Type: gvtrademap_cs.LoadInfosStatus
// Assembly: gvtrademap_cs, Version=1.3.2.3, Culture=neutral, PublicKeyToken=null
// MVID: 3D162A44-1A8B-4B7A-9FC3-6379559CB419
// Assembly location: C:\tmp\A\files\gvtrademap_cs.exe

namespace gvtrademap_cs
{
  public class LoadInfosStatus
  {
    public int NowStep { get; set; }

    public int MaxStep { get; set; }

    public string StatusMessage { get; set; }

    public void Start(int max, string message)
    {
      MaxStep = max;
      NowStep = 0;
      StatusMessage = message;
    }

    public void IncStep(string next_message)
    {
      StatusMessage = next_message;
      if (++NowStep < MaxStep)
        return;
      NowStep = MaxStep;
      StatusMessage = "完了";
    }
  }
}
