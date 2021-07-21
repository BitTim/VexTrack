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
		private HistoryView View { get; set; }

		public void Update()
		{
			if (View == null) return;

			if (View.AreCommandsSet == false)
			{
				View.SetCommands(OnHistoryButtonClick);
			}

			List<HistoryEntry> history = TrackingDataHelper.Data.Seasons.Last<Season>().History;
			foreach (HistoryEntry he in history)
			{
				string result = HistoryDataCalc.CalcHistoryResult(he.Description);

				string backgroundKey = "";
				if (result == "Win" || result == "Loss") backgroundKey = result + "Color";

				string foregroundKey = "";
				if (result == "Win" || result == "Loss") foregroundKey = "White";

				View.AddHistoryEntryButton(he.Description, he.Amount, backgroundKey, foregroundKey);
			}
		}

		public void RegisterView(HistoryView view)
		{
			View = view;
			Update();
		}

		public void OnHistoryButtonClick(object parameter)
		{
			int index = (int)parameter;

			//TODO: Add Info Card view and fill with info for this entry
		}
	}
}
