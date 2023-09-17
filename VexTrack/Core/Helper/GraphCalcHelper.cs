using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using VexTrack.Core.Model;

namespace VexTrack.Core.Helper;

public static class GraphCalcHelper
{
	public static SeriesCollection CalcGraphs(string seasonUuid)
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
			LineSmoothness = 0
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

		var seasonData = UserData.Seasons.FirstOrDefault(s => s.Uuid == seasonUuid);
		if (seasonData == null) return collection;
		
		var startRemaining = seasonData.Total - seasonData.StartXp;
		var totalDaily = startRemaining / (double)(seasonData.Duration - seasonData.BufferDays);
		var dailyAmounts = HistoryHelper.CalcDailyCollectedFromSeason(seasonUuid);
		
		var prevPerformanceVal = 0;
			
		for (var i = 0; i < seasonData.Duration + 1; i++)
		{
			// Ideal
			var idealVal = (int)Math.Ceiling(i * totalDaily + seasonData.StartXp);
			if (idealVal > seasonData.Total) idealVal = seasonData.Total;

			idealSeries.Values.Add(new ObservablePoint(i, idealVal));

			// Performance
			if(i >= seasonData.Duration - seasonData.RemainingDays + 1) continue;
				
			var performanceVal = prevPerformanceVal + dailyAmounts[i];
			prevPerformanceVal = performanceVal;

			performanceSeries.Values.Add(new ObservablePoint(i, performanceVal + seasonData.StartXp));		// Offset Performance by StartXp
		}

		collection.Add(idealSeries);
		collection.Add(performanceSeries);
			
		return collection;
	}

	public static SeriesCollection CalcDailyGraphs(int dayIndex, int dailyIdeal, string seasonUuid)
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
			StrokeDashArray = new DoubleCollection { 2, 2 }
		};
			
		var averageSeries = new LineSeries
		{
			Title = "Average",
			Values = new ChartValues<ObservablePoint>(),
			PointGeometry = null,
			Stroke = loss,
			Fill = Brushes.Transparent,
			LineSmoothness = 0,
			StrokeDashArray = new DoubleCollection { 1, 1 }
		};
		
		var seasonData = UserData.Seasons.FirstOrDefault(s => s.Uuid == seasonUuid);
		if (seasonData == null) return collection;

		var dailyAmounts = HistoryHelper.CalcDailyCollectedFromSeason(seasonUuid);
		var collected = dailyAmounts.GetRange(0, dayIndex).Sum(); 

		var offset = dayIndex - 1;
		for (var i = offset; i < seasonData.Duration + 1; i++)
		{
			// Daily Ideal
			var dailyIdealVal = collected + (int)MathF.Ceiling((i - offset) * dailyIdeal) + seasonData.StartXp;	// Offset DailyIdeal by StartXp
			if (dailyIdealVal > seasonData.Total) dailyIdealVal = seasonData.Total;

			dailyIdealSeries.Values.Add(new ObservablePoint(i, dailyIdealVal));

			// Average
			var averageVal = collected + (int)MathF.Ceiling((i - offset) * seasonData.Average) + seasonData.StartXp;	// Offset Average by StartXp
			averageSeries.Values.Add(new ObservablePoint(i, averageVal));
		}
			
		collection.Add(dailyIdealSeries);
		collection.Add(averageSeries);

		return collection;
	}
}