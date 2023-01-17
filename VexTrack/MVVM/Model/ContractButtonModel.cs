using System.Collections.ObjectModel;
using System.Windows.Controls;
using VexTrack.Core;

namespace VexTrack.MVVM.Model;

public class ContractButtonModel : Button
{
    private Contract RawData { get; set; }
    public string Title { get; set; }
    public string Color { get; set; }
    public ObservableCollection<Goal> Goals { get; set; }
    
    // TODO: Make these DependencyProperties
    public string Uuid { get; set; }
    public string Name { get; set; }
    public int Collected { get; set; }
    public int Total { get; set; }
    public int Remaining { get; set; }
    public double Progress { get; set; }
}