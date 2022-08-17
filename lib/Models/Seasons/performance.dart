import 'dart:math';

import 'package:fl_chart/fl_chart.dart';
import 'package:vextrack/Core/history_calc.dart';
import 'package:vextrack/Models/History/history_entry_group.dart';
import 'package:vextrack/Models/Seasons/season.dart';
import 'package:vextrack/Models/Seasons/season_meta.dart';
import 'package:vextrack/Services/settings.dart';

class Performance
{
  int duration;
  int total;
  int epilogueTotal;
  int avgDailyXP;
  int activeXP;
  SeasonMeta meta;
  List<HistoryEntryGroup> history;

  bool cumulative = true;
  bool epilogue = false;

  Performance({
    required this.duration,
    required this.total,
    required this.epilogueTotal,
    required this.avgDailyXP,
    required this.activeXP,
    required this.meta,
    required this.history,
  });

  static Performance fromSeason(Season season)
  {
    return Performance(
      duration: season.meta.getDuration().inDays,
      total: season.getTotal(),
      epilogueTotal: season.getTotal() + season.getEpilogueTotal(),
      avgDailyXP: season.getDailyAvg(),
      activeXP: season.getXP(),
      meta: season.meta,
      history: season.history,
    );
  }



  // --------------------------------
  // Chart data
  // --------------------------------

  List<Point> getAverageIdeal()
  {
    int effectiveDuration = duration - SettingsService.bufferDays;
    int localTotal = epilogue ? epilogueTotal : total;
    double dailyXP = localTotal / effectiveDuration;

    List<Point> points = [];
    int cumulativeSum = 0;

    for (int i = 0; i < duration; i++)
    {
      int amount = 0;

      if (i < effectiveDuration)
      {
        amount = dailyXP.ceil();
        cumulativeSum += amount;
        if (cumulativeSum > localTotal) cumulativeSum = localTotal;
      }

      points.add(Point(i, cumulative ? cumulativeSum : amount));
    }

    return points;
  }

  List<Point> getUserXP()
  {
    List<Point> points = [];
    List<int> dailyXP = HistoryCalc.getXPPerDay(history, meta);
    double cumulativeSum = 0;

    for (int i = 0; i < dailyXP.length; i++)
    {
      double amount = dailyXP[i].toDouble();
      cumulativeSum += amount;
      points.add(Point(i, cumulative ? cumulativeSum : amount));
    }

    return points;
  }

  List<Point> getUserAverage()
  {
    List<Point> points = [];
    int cumulativeSum = 0;

    for (int i = 0; i < duration; i++)
    {
      cumulativeSum += avgDailyXP;
      points.add(Point(i, cumulative ? cumulativeSum : avgDailyXP));
    }

    return points;
  }

  List<Point> getUserIdeal()
  {
    if (!meta.isActive()) return [];

    List<Point> points = [];
    return points;
  }




  // --------------------------------
  // Util
  // --------------------------------

  static List<FlSpot> mapPointToSpot(List<Point> points)
  {
    return points.map((e) => FlSpot(e.x.toDouble(), e.y.toDouble())).toList();
  }

  int getMaxChartXP()
  {
    if (cumulative)
    {
      if (total > activeXP) return total;
      return activeXP;
    }
    else
    {
      int maxXP = getUserXP().map((e) => e.y).reduce(max).toInt();
      int maxIdeal = getAverageIdeal()[0].y.toInt();
      if (maxIdeal > maxXP) return maxIdeal;
      return maxXP;
    }
  }
}