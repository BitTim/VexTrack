using System;
using System.Collections.Generic;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;
using LineSeries = LiveCharts.Wpf.LineSeries;

namespace VexTrack.MVVM.ViewModel
{
	class HomeViewModel : ObservableObject
	{
		public RelayCommand OnAddClicked { get; private set; }
		private EditableHistoryEntryPopupViewModel EditableHePopup { get; set; }
		private MainViewModel MainVm { get; set; }

		private string _title;
		private string _username;
		private int _collected;
		private int _remaining;
		private int _total;
		private int _streak;
		private Brush _streakColor;
		private double _progress;
		private string _seasonName;
		private int _deviationIdeal;
		private int _deviationDaily;
		private int _daysFinished;
		private int _daysRemaining;
		private SeriesCollection _graphSeriesCollection;
		private List<int> _segments;

		public string Title
		{
			get => _title;
			private set
			{
				_title = value;
				OnPropertyChanged();
			}
		}

		public string Username
		{
			get => _username;
			private set
			{
				_username = value;
				OnPropertyChanged();
			}
		}

		public int Collected
		{
			get => _collected;
			private set
			{
				_collected = value;
				OnPropertyChanged();
			}
		}

		public int Remaining
		{
			get => _remaining;
			set
			{
				_remaining = value;
				OnPropertyChanged();
			}
		}

		public int Total
		{
			get => _total;
			set
			{
				_total = value;
				OnPropertyChanged();
			}
		}

		public int Streak
		{
			get => _streak;
			private set
			{
				_streak = value;
				OnPropertyChanged();
			}
		}

		public Brush StreakColor
		{
			get => _streakColor;
			private set
			{
				_streakColor = value;
				OnPropertyChanged();
			}
		}

		public double Progress
		{
			get => _progress;
			set
			{
				_progress = value;
				OnPropertyChanged();
			}
		}

		public string SeasonName
		{
			get => _seasonName;
			private set
			{
				_seasonName = value;
				OnPropertyChanged();
			}
		}

		public int DeviationIdeal
		{
			get => _deviationIdeal;
			set
			{
				_deviationIdeal = value;
				OnPropertyChanged();
			}
		}

		public int DeviationDaily
		{
			get => _deviationDaily;
			set
			{
				_deviationDaily = value;
				OnPropertyChanged();
			}
		}

		public int DaysFinished
		{
			get => _daysFinished;
			private set
			{
				_daysFinished = value;
				OnPropertyChanged();
			}
		}

		public int DaysRemaining
		{
			get => _daysRemaining;
			private set
			{
				_daysRemaining = value;
				OnPropertyChanged();
			}
		}

		public SeriesCollection GraphSeriesCollection
		{
			get => _graphSeriesCollection;
			set
			{
				_graphSeriesCollection = value;
				OnPropertyChanged();
			}
		}

		private List<int> Segments
		{
			get => _segments;
			set
			{
				_segments = value;
				OnPropertyChanged();
			}
		}

		public string Status => GetStatus();
		public List<decimal> LogicalStops => CalcUtil.CalcLogicalStops(Segments, true);
		public List<decimal> VisualStops => CalcUtil.CalcVisualStops(Segments, true);

		
		
		public HomeViewModel()
		{
			MainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
			EditableHePopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels[nameof(EditableHistoryEntryPopupViewModel)];
			
			Update();
		}

		public void Update()
		{
			Username = SettingsHelper.Data.Username;
			Title = Username != "" ? "Welcome back," : "Welcome back";

			var data = HomeDataCalc.CalcDailyData();

			Collected = data.Collected;
			Remaining = data.Remaining;
			Total = data.Total;
			Progress = data.Progress;
			Streak = data.Streak;
			Segments = data.Segments;

			StreakColor = TrackingData.LastStreakUpdateTimestamp == ((DateTimeOffset)DateTimeOffset.Now.ToLocalTime().Date).ToUnixTimeSeconds()
				? SettingsHelper.Data.Theme.AccentBrush
				: SettingsHelper.Data.Theme.ShadeBrush;

			SeasonName = TrackingData.CurrentSeasonData.Name;
			(DeviationIdeal, DeviationDaily) = CalcGraph();
			DaysRemaining = TrackingData.CurrentSeasonData.RemainingDays;
			DaysFinished = HomeDataCalc.CalcDaysFinished(true);

			OnAddClicked = new RelayCommand(_ =>
			{
				EditableHePopup.SetParameters("Create History Entry", false);
				MainVm.QueuePopup(EditableHePopup);
			});
		}

		private (int, int) CalcGraph() // TODO: Refactor this
		{
			var currSeason = TrackingData.CurrentSeasonData;
			var seriesCollection = currSeason.GraphSeriesCollection;

			var ideal = seriesCollection[0] as LineSeries;
			var performance = seriesCollection[1] as LineSeries;
			
			var dailyIdeal = HomeDataCalc.CalcDailyIdeal(performance);
			var average = HomeDataCalc.CalcAverageGraph(performance);
			
			seriesCollection.Add(dailyIdeal);
			seriesCollection.Add(average);

			GraphSeriesCollection = seriesCollection;

			var t = performance.Values.Count - 1;
			var deviationIdeal = (int)((ObservablePoint)performance.Values[t]).Y - (int)((ObservablePoint)ideal.Values[t]).Y;

			var deviationDaily = 0;
			if (dailyIdeal.Values.Count > 0) deviationDaily = (int)((ObservablePoint)performance.Values[t]).Y - (int)((ObservablePoint)dailyIdeal.Values[1]).Y;

			return (deviationIdeal, deviationDaily);
		}
		
		private string GetStatus()
		{
			if (Collected < Segments[0]) return "Warning";
			if (Collected >= Segments[0] && Collected < Total) return "Done";
			return Collected >= Total ? "DoneAll" : "";
		}
		
		
		
		
		
		
		
		
		
		
		
		
		public int BufferDays => TrackingData.CurrentSeasonData.BufferDays;
		public int BufferDaysPosition => TrackingData.CurrentSeasonData.Duration - BufferDays;
		
		public Func<double, string> LargeNumberFormatter => value => //TODO Move to own class
		{
			if (value == 0) return "0";
        
			var mag = (int)(Math.Floor(Math.Log10(value)) / 3); // Truncates to 6, divides to 2
			var divisor = Math.Pow(10, mag * 3);

			var shortNumber = value / divisor;

			var suffix = mag switch
			{
				0 => string.Empty,
				1 => "k",
				2 => "M",
				3 => "B",
				_ => ""
			};

			return shortNumber.ToString("N1") + suffix;
		};
	}
}
