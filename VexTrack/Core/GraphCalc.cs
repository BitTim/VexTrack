using System;
using System.Collections.Generic;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace VexTrack.Core
{
	public static class GraphCalc
	{
		public static SeriesCollection CalcGraphs(int total, int startXp, int duration, int bufferDays, int remainingDays, List<HistoryEntry> history)
		{
			var collection = new SeriesCollection();

			var mono = SettingsHelper.Data.Theme.MonoBrush ?? new SolidColorBrush();
			var accent = SettingsHelper.Data.Theme.AccentBrush ?? new SolidColorBrush();

			var ideal = new LineSeries
			{
				Title = "Ideal",
				Values = new ChartValues<ObservablePoint>(),
				PointGeometry = null,
				Stroke = mono,
				Fill = Brushes.Transparent,
				LineSmoothness = 0,
			};
			
			var performance = new LineSeries
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
			var dailyAmounts = CalcUtil.CalcCollectedPerDay(history, duration);

			var prevPerformanceVal = 0;
			
			for (var i = 0; i < duration; i++)
			{
				// Ideal
				var idealVal = (int)Math.Ceiling(i * totalDaily + startXp);
				if (idealVal > total) idealVal = total;

				ideal.Values.Add(new ObservablePoint(i, idealVal));

				// Performance
				if(i >= duration - remainingDays + 1) continue;
				
				var performanceVal = prevPerformanceVal + dailyAmounts[i];
				prevPerformanceVal = performanceVal;

				performance.Values.Add(new ObservablePoint(i, performanceVal));
			}

			collection.Add(ideal);
			collection.Add(performance);
			
			return collection;
		}
	}
}
