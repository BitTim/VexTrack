namespace VexTrack.Core.Model;

public class GoalTemplate
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public int Total { get; set; }

    public GoalTemplate(string uuid, string name, int total)
    {
        Uuid = uuid;
        Name = name;
        Total = total;
    }
}