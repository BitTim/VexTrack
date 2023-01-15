using System.Collections.ObjectModel;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Controls;

public class ContractButtonControlViewModel : ObservableObject
{
    private Contract RawData { get; set; }
    public string Title { get; set; }
    public string Color { get; set; }
    public ObservableCollection<Goal> Goals { get; set; }
    
    
}