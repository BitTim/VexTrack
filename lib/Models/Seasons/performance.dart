import 'dart:math';

import 'package:fl_chart/fl_chart.dart';
import 'package:vextrack/Core/history_calc.dart';
import 'package:vextrack/Models/History/history_entry.dart';
import 'package:vextrack/Models/Seasons/season.dart';
import 'package:vextrack/Services/settings.dart';

class Performance
{
  int duration;
  int total;
  int epilogueTotal;
  bool isActive;
  List<HistoryEntry> history;

  bool cumulative = true;
  bool epilogue = false;

  Performance({
    required this.duration,
    required this.total,
    required this.epilogueTotal,
    required this.isActive,
    required this.history,
  });

  static Performance fromSeason(Season season)
  {
    return Performance(
      duration: season.meta.getDuration().inDays,
      total: season.getTotal(),
      epilogueTotal: season.getTotal() + season.getEpilogueTotal(),
      isActive: season.isActive(),
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
    List<int> dailyXP = HistoryCalc.getXPPerDay(history, isActive, duration);
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
    List<int> actualDailyXP = HistoryCalc.getXPPerDay(history, isActive, duration);
    int effectiveDuration = actualDailyXP.length;

    int activeXP = 0;
    for (int i = 0; i < effectiveDuration; i++) { activeXP += actualDailyXP[i]; }

    double avgDailyXP = activeXP / effectiveDuration;

    List<Point> points = [];
    double cumulativeSum = 0;

    for (int i = 0; i < duration; i++)
    {
      double amount = 0;

      if (i < effectiveDuration)
      {
        amount = avgDailyXP;
        cumulativeSum += amount;
      }

      points.add(Point(i, cumulative ? cumulativeSum : amount));
    }

    return points;
  }

  List<Point>? getUserIdeal()
  {
    if (!isActive) { return null; }


  }




  // --------------------------------
  // Util
  // --------------------------------

  static List<FlSpot> mapPointToSpot(List<Point> points)
  {
    return points.map((e) => FlSpot(e.x.toDouble(), e.y.toDouble())).toList();
  }
}