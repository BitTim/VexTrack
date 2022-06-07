using System.Windows;
using System.Windows.Controls;

namespace VexTrack.MVVM.Model
{
	public class HistoryEntryButtonModel : Button
	{
		public static DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(HistoryEntryButtonModel), new PropertyMetadata(""));
		public static DependencyProperty AmountProperty = DependencyProperty.Register("Amount", typeof(string), typeof(HistoryEntryButtonModel), new PropertyMetadata("0"));

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
}
