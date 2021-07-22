using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
    class EditableHistoryEntryPopupViewModel
    {
    		public string Title { get; set; }
    
    		<!-- Add OnPropertyChanged to setters -->
    
        public string Description { get; set; }
		public long Time { get; set; }
		public int Amount { get; set; }
		public string Map { get; set; }
		public string Result { get; set; }

		private MainViewModel MainVM { get; set; }

		public HistoryEntryPopupViewModel()
		{
			MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
		}
		
		public void SetTile(string title)
		{
			Title = title;
		}
    }
}