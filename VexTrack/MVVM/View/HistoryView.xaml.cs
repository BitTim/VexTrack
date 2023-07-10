using VexTrack.MVVM.ViewModel;

namespace VexTrack.MVVM.View;

/// <summary>
/// Interaction logic for HistoryView.xaml
/// </summary>
public partial class HistoryView
{
	public HistoryView()
	{
		InitializeComponent();
		DataContext = ViewModelManager.ViewModels[nameof(HistoryViewModel)];
	}
}