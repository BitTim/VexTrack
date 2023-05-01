using VexTrack.Core.Helper;

namespace VexTrack.Core.Model;

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

    private int GetProgress() { return CalcHelper.CalcProgress(Total, Collected); }
    private int GetRemaining() { return Total - Collected; }
    public bool IsCompleted() { return Collected >= Total; }
}