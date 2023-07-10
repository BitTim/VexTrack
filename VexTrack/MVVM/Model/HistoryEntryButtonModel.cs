using System.Windows;
using System.Windows.Controls;

namespace VexTrack.MVVM.Model;

public class HistoryEntryButtonModel : Button
{
	public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(HistoryEntryButtonModel), new PropertyMetadata(""));
	public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(nameof(Amount), typeof(string), typeof(HistoryEntryButtonModel), new PropertyMetadata("0"));

	public string Description
	{
		get => (string)GetValue(DescriptionProperty);
		set => SetValue(DescriptionProperty, value);
	}

	public string Amount
	{
		get => (string)GetValue(AmountProperty);
		set => SetValue(AmountProperty, value);
	}

	static HistoryEntryButtonModel()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(HistoryEntryButtonModel), new FrameworkPropertyMetadata(typeof(HistoryEntryButtonModel)));
	}

	public HistoryEntryButtonModel() { }
	public HistoryEntryButtonModel(string description, int amount)
	{
		Description = description;
		Amount = amount.ToString();
	}
}