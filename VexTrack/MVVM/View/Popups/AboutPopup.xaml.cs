﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VexTrack.MVVM.View.Popups
{
	/// <summary>
	/// Interaction logic for AboutPopup.xaml
	/// </summary>
	public partial class AboutPopup : UserControl
	{
		public AboutPopup()
		{
			InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) {UseShellExecute = true});
			e.Handled = true;
		}
	}
}