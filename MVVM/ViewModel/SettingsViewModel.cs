using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel
{
	class SettingsViewModel : ObservableObject
	{
		public RelayCommand ThemeButtonCommand { get; set; }
		public RelayCommand AccentButtonCommand { get; set; }

		public string Theme { get; set; }
		public string Accent { get; set; }

		public SettingsViewModel()
		{
			ThemeButtonCommand = new RelayCommand(theme => SetTheme((string)theme));
			AccentButtonCommand = new RelayCommand(accent => SetAccent((string)accent));
		}

		public void Update()
		{

		}

		public void SetTheme(string theme)
		{
			Theme = theme;
			if (theme == "Auto") return;

			Application.Current.Resources.MergedDictionaries[1].Source = new Uri(Constants.ThemeURIs[theme], UriKind.Relative);
			if (Accent == "Mono") SetAccent("Mono");
		}

		public void SetAccent(string accent)
		{
			Accent = accent;
			Application.Current.Resources.MergedDictionaries[2].Source = new Uri(Constants.AccentURIs[accent], UriKind.Relative);
		}
	}
}
