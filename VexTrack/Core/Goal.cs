namespace VexTrack.Core;

public class Goal
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public int Total { get; set; }
    public int Collected { get; set; }


    public double Progress => GetProgress();
    
    
    public Goal(string uuid, string name, int total, int collected)
    {
        Uuid = uuid;
        Name = name;
        Total = total;
        Collected = collected;
    }

    public double GetProgress()
    {
        return CalcUtil.CalcProgress(Total, Collected);
    }
}