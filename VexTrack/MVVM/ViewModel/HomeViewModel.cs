using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using OxyPlot.Legends;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;
using AxisPosition = OxyPlot.Axes.AxisPosition;
using LineSeries = LiveCharts.Wpf.LineSeries;

namespace VexTrack.MVVM.ViewModel
{
	internal class HomeViewModel : ObservableObject
	{
		public RelayCommand OnAddClicked { get; set; }
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
		//private readonly PlotModel _graph;
		private SeriesCollection _graphSeriesCollection;

		public string Title
		{
			get => _title;
			set
			{
				_title = value;
				OnPropertyChanged();
			}
		}

		public string Username
		{
			get => _username;
			set
			{
				_username = value;
				OnPropertyChanged();
			}
		}

		public int Collected
		{
			get => _collected;
			set
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
			set
			{
				_streak = value;
				OnPropertyChanged();
			}
		}

		public Brush StreakColor
		{
			get => _streakColor;
			set
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
			set
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
			set
			{
				_daysFinished = value;
				OnPropertyChanged();
			}
		}

		public int DaysRemaining
		{
			get => _daysRemaining;
			set
			{
				_daysRemaining = value;
				OnPropertyChanged();
			}
		}

		// public PlotModel Graph
		// {
		// 	get => _graph;
		// 	init
		// 	{
		// 		_graph = value;
		// 		OnPropertyChanged();
		// 	}
		// }

		public SeriesCollection GraphSeriesCollection
		{
			get => _graphSeriesCollection;
			set
			{
				_graphSeriesCollection = value;
				OnPropertyChanged();
			}
		}

		public List<decimal> Stops => new List<decimal>() { (decimal)0.8, 1 };

		public HomeViewModel()
		{
			Update(false);
		}

		public void Update(bool epilogue)
		{
			MainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
			EditableHePopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels["EditableHEPopup"];

			Username = SettingsHelper.Data.Username;
			Title = Username != "" ? "Welcome back," : "Welcome back";

			var data = DashboardDataCalc.CalcDailyData(epilogue);

			Collected = data.Collected;
			Remaining = data.Remaining;
			Total = data.Total;
			Progress = data.Progress;
			Streak = data.Streak;
			StreakColor = StreakDataCalc.GetStreakColor(DateTimeOffset.Now.ToLocalTime().Date, epilogue);

			SeasonName = TrackingData.CurrentSeasonData.Name;
			(DeviationIdeal, DeviationDaily) = CalcGraph(epilogue);
			DaysRemaining = TrackingData.GetRemainingDays(TrackingData.CurrentSeasonData.Uuid);
			DaysFinished = DashboardDataCalc.CalcDaysFinished(epilogue);

			OnAddClicked = new RelayCommand(o =>
			{
				EditableHePopup.SetParameters("Create History Entry", false);
				MainVm.QueuePopup(EditableHePopup);
			});
		}

		private (int, int) CalcGraph(bool epilogue)
		{
			var currSeason = TrackingData.CurrentSeasonData;
			var seriesCollection = currSeason.GraphSeriesCollection;

			var ideal = seriesCollection[0] as LineSeries;
			var performance = seriesCollection[1] as LineSeries;
			
			var dailyIdeal = DashboardDataCalc.CalcDailyIdeal(performance);
			var average = DashboardDataCalc.CalcAverageGraph(performance);
			
			seriesCollection.Add(dailyIdeal);
			seriesCollection.Add(average);

			GraphSeriesCollection = seriesCollection;

			var t = performance.Values.Count - 1;
			var deviationIdeal = (int)((ObservablePoint)performance.Values[t]).Y - (int)((ObservablePoint)ideal.Values[t]).Y;

			var deviationDaily = 0;
			if (dailyIdeal.Values.Count > 0) deviationDaily = (int)((ObservablePoint)performance.Values[t]).Y - (int)((ObservablePoint)dailyIdeal.Values[1]).Y;

			return (deviationIdeal, deviationDaily);
		}
		
		
		
		
		
		
		
		
		
		
		
		
		public int BufferDays => SettingsHelper.Data.BufferDays;
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
