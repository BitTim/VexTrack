using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VexTrack.Core;
using VexTrack.MVVM.Model;
using VexTrack.MVVM.View;

namespace VexTrack.MVVM.ViewModel
{
	class HistoryViewModel : ObservableObject
	{
		HistoryView View { get; set; }

		public void Update()
		{
			if (View == null) return;

			List<HistoryEntry> history = TrackingDataHelper.Data.Seasons.Last<Season>().History;
			foreach (HistoryEntry he in history)
			{
				View.AddHistoryEntryButton(he.Description, he.Amount);
			}
		}

		public void RegisterView(HistoryView view)
		{
			View = view;
			Update();
		}
	}
}
