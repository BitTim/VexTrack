using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VexTrack.MVVM.Converter;

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

		private TextBlock DescriptionTextBlock { get; set; }
		private TextBlock AmountTextBlock { get; set; }

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

		public override void OnApplyTemplate()
		{
			DescriptionTextBlock = (TextBlock)Template.FindName("PART_DescriptionTextBlock", this);
			AmountTextBlock = (TextBlock)Template.FindName("PART_AmountTextBlock", this);

			base.OnApplyTemplate();
		}
	}
}
