using VexTrack.Core.Helper;
using VexTrack.Core.Model.Game.Templates;

namespace VexTrack.Core.Model;

public class Goal
{
    public GoalTemplate Template { get; set; }
    public string Uuid { get; set; }
    public int Collected { get; set; }

    
    public string Name => Template.Name;
    public int Total => Template.XpTotal;
    public bool IsEpilogue => Template.IsEpilogue;
    

    public int Progress => GetProgress();
    public int Remaining => GetRemaining();


    public Goal(GoalTemplate template, string uuid, int collected)
    {
        Template = template;
        Uuid = uuid;
        Collected = collected;
    }

    private int GetProgress() { return CalcHelper.CalcProgress(Template.XpTotal, Collected); }
    private int GetRemaining() { return Template.XpTotal - Collected; }
    public bool IsCompleted() { return Collected >= Template.XpTotal; }
}