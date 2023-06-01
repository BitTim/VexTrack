using VexTrack.MVVM.ViewModel;

namespace VexTrack.MVVM.View;

/// <summary>
/// Interaction logic for ContractView.xaml
/// </summary>
public partial class ContractView
{
	public ContractView()
	{
		InitializeComponent();
		DataContext = ViewModelManager.ViewModels[nameof(ContractViewModel)];
	}
}