namespace VexTrack.Core.Model.Game.Cosmetic;

public class Spray : Cosmetic 
{
    public string IconPath { get; set; }
    public string FullIconPath { get; set; }
    public string AnimationPath { get; set; }
    
    public bool IsAnimated { get; set; }

    public Spray(string uuid, string name, string iconPath, string fullIconPath, string animationPath) : base(uuid, name, "Spray")
    {
        IconPath = iconPath;
        FullIconPath = fullIconPath;
        AnimationPath = animationPath;

        IsAnimated = !string.IsNullOrEmpty(animationPath);
    }
}