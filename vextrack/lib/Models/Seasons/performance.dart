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
  int activeDay;
  int userIdeal;
  int userEpilogueIdeal;
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
    required this.activeDay,
    required this.userIdeal,
    required this.userEpilogueIdeal,
    required this.meta,
    required this.history,
    required this.cumulative,
    required this.epilogue,
  });

  static Performance fromSeason(Season season, bool cumulativeInit, bool epilogueInit)
  {
    return Performance(
      duration: season.meta.getDuration().inDays,
      total: season.getTotal(),
      epilogueTotal: season.getTotal() + season.getEpilogueTotal(),
      avgDailyXP: season.getDailyAvg(),
      activeXP: season.getXP(),
      activeDay: season.getDaysIndexFromDate(DateTime.now()),
      userIdeal: season.getUserIdeal(),
      userEpilogueIdeal: season.getUserEpilogueIdeal(),
      meta: season.meta,
      history: season.history,
      cumulative: cumulativeInit,
      epilogue: epilogueInit,
    );
  }



  // --------------------------------
  // Chart data
  // --------------------------------

  List<Point> getIdeal()
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
    int localTotal = epilogue ? epilogueTotal : total;
    int cumulativeSum = 0;

    int limit = localTotal > activeXP ? localTotal : activeXP;
    bool firstOver = true;

    for (int i = 0; i < duration; i++)
    {
      cumulativeSum += avgDailyXP;
      if(cumulativeSum > limit)
      {
        if (!firstOver) break;
        firstOver = false;
      }
      points.add(Point(i, cumulative ? cumulativeSum : avgDailyXP));
    }

    return points;
  }

  List<Point> getUserIdeal()
  {
    List<Point> points = [];
    List<int> dailyXP = HistoryCalc.getXPPerDay(history, meta);
    int localUserIdeal = epilogue ? userEpilogueIdeal : userIdeal;
    int localTotal = epilogue ? epilogueTotal : total;
    int cumulativeSum = 0;
    bool firstOver = true;

    for(int i = 0; i <= activeDay - 1; i++)
    {
      cumulativeSum += dailyXP[i];
    }

    points.add(Point(activeDay - 1, cumulative ? cumulativeSum : localUserIdeal));

    for (int i = activeDay; i < duration; i++)
    {
      cumulativeSum += localUserIdeal;
      if(cumulativeSum > localTotal)
      {
        if (!firstOver) break;
        firstOver = false;
      }
      points.add(Point(i, cumulative ? cumulativeSum : localUserIdeal));
    }

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
      int maxIdeal = getIdeal()[0].y.toInt();
      if (maxIdeal > maxXP) return maxIdeal;
      return maxXP;
    }
  }
}