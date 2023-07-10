namespace VexTrack.Core.Model.Game.Cosmetic;

public class PlayerTitle : Cosmetic
{
    public string TitleText { get; set; }

    public PlayerTitle(string uuid, string name, string titleText) : base(uuid, name, "Title")
    {
        TitleText = titleText;
    }
}