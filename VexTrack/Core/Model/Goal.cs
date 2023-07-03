using VexTrack.Core.Helper;

namespace VexTrack.Core.Model;

public class Goal
{
    public GoalTemplate Template { get; set; }
    public int Collected { get; set; }


    public string Uuid => Template.Uuid;
    public string Name => Template.Name;
    public int Total => Template.Total;
    

    public int Progress => GetProgress();
    public int Remaining => GetRemaining();


    public Goal(GoalTemplate template, int collected)
    {
        Template = template;
        Collected = collected;
    }

    private int GetProgress() { return CalcHelper.CalcProgress(Template.Total, Collected); }
    private int GetRemaining() { return Template.Total - Collected; }
    public bool IsCompleted() { return Collected >= Template.Total; }
}