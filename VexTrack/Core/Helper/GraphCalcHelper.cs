using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace VexTrack.Core.Helper;

public static class GraphCalcHelper
{
	public static SeriesCollection CalcGraphs(int total, int startXp, long startDate,  int duration, int bufferDays, int remainingDays, string seasonUuid)
	{
		var collection = new SeriesCollection();

		var mono = SettingsHelper.Data.Theme.MonoBrush ?? new SolidColorBrush();
		var accent = SettingsHelper.Data.Theme.AccentBrush ?? new SolidColorBrush();

		var idealSeries = new LineSeries
		{
			Title = "Ideal",
			Values = new ChartValues<ObservablePoint>(),
			PointGeometry = null,
			Stroke = mono,
			Fill = Brushes.Transparent,
			LineSmoothness = 0,
		};
			
		var performanceSeries = new LineSeries
		{
			Title = "Performance",
			Values = new ChartValues<ObservablePoint>(),
			PointGeometry = null,
			Stroke = accent,
			StrokeThickness = 3,
			Fill = Brushes.Transparent,
			LineSmoothness = 0
		};
			
		var startRemaining = total - startXp;
		var totalDaily = startRemaining / (double)(duration - bufferDays);
		var dailyAmounts = CalcHelper.CalcCollectedPerDay(startDate, HistoryHelper.GetAllEntriesFromSeason(seasonUuid), duration);

		var prevPerformanceVal = 0;
			
		for (var i = 0; i < duration + 1; i++)
		{
			// Ideal
			var idealVal = (int)Math.Ceiling(i * totalDaily + startXp);
			if (idealVal > total) idealVal = total;

			idealSeries.Values.Add(new ObservablePoint(i, idealVal));

			// Performance
			if(i >= duration - remainingDays + 1) continue;
				
			var performanceVal = prevPerformanceVal + dailyAmounts[i];
			prevPerformanceVal = performanceVal;

			performanceSeries.Values.Add(new ObservablePoint(i, performanceVal));
		}

		collection.Add(idealSeries);
		collection.Add(performanceSeries);
			
		return collection;
	}

	public static SeriesCollection CalcDailyGraphs(int total, int dayIndex, long startDate, int duration, int dailyIdeal, int average, string seasonUuid)
	{
		var collection = new SeriesCollection();

		var win = (Brush)Application.Current.FindResource("Win") ?? new SolidColorBrush();
		var loss = (Brush)Application.Current.FindResource("Loss") ?? new SolidColorBrush();
			
		var dailyIdealSeries = new LineSeries
		{
			Title = "Daily Ideal",
			Values = new ChartValues<ObservablePoint>(),
			PointGeometry = null,
			Stroke = win,
			Fill = Brushes.Transparent,
			LineSmoothness = 0,
			StrokeDashArray = new DoubleCollection { 2, 2 },
		};
			
		var averageSeries = new LineSeries
		{
			Title = "Average",
			Values = new ChartValues<ObservablePoint>(),
			PointGeometry = null,
			Stroke = loss,
			Fill = Brushes.Transparent,
			LineSmoothness = 0,
			StrokeDashArray = new DoubleCollection { 1, 1 },
		};

		var dailyAmounts = CalcHelper.CalcCollectedPerDay(startDate, HistoryHelper.GetAllEntriesFromSeason(seasonUuid), duration);
		var collected = dailyAmounts.GetRange(0, dayIndex).Sum(); //TODO: "Out of Bounds" since no new season is created at the moment. Fix with #69 

		var offset = dayIndex - 1;
		for (var i = offset; i < duration + 1; i++)
		{
			// Daily Ideal
			var dailyIdealVal = collected + (int)MathF.Ceiling((i - offset) * dailyIdeal);
			if (dailyIdealVal > total) dailyIdealVal = total;

			dailyIdealSeries.Values.Add(new ObservablePoint(i, dailyIdealVal));

			// Average
			var averageVal = collected + (int)MathF.Ceiling((i - offset) * average);
			averageSeries.Values.Add(new ObservablePoint(i, averageVal));
		}
			
		collection.Add(dailyIdealSeries);
		collection.Add(averageSeries);

		return collection;
	}
}