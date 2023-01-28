namespace VexTrack.Core;

public class Goal
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public int Total { get; set; }
    public int Collected { get; set; }


    public int Progress => GetProgress();
    public int Remaining => GetRemaining();
    
    
    public Goal(string uuid, string name, int total, int collected)
    {
        Uuid = uuid;
        Name = name;
        Total = total;
        Collected = collected;
    }

    public int GetProgress() { return CalcUtil.CalcProgress(Total, Collected); }
    public int GetRemaining() { return Total - Collected; }
    public bool IsCompleted() { return Collected >= Total; }
}