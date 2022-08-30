import 'package:vextrack/Models/History/history_entry.dart';
import 'package:vextrack/Models/History/history_entry_group.dart';
import 'package:vextrack/Models/Seasons/season_meta.dart';

class HistoryCalc
{
  static List<int> getXPPerDay(List<HistoryEntryGroup> history, SeasonMeta meta) {
    List<int> amounts = [0];
    DateTime prevDate = history[0].getDate();

    for (HistoryEntryGroup heg in history)
    {
      DateTime cDate = heg.getDate();
      for (int i = 0; cDate.difference(prevDate).inDays - 1 > i; i++) {
        amounts.add(0);
      }

      amounts.add(heg.total);
      prevDate = cDate;
    }

    if (!meta.isActive()) // Fill missing entries for completed seasons
    {
      int missingEntries = meta.getDuration().inDays - amounts.length;
      
      for (int i = missingEntries; i > 0; i--)
      {
        amounts.add(0);
      }
    }
    else // Fill entries up until current day for active season
    {
      int missingEntries = DateTime.now().difference(prevDate).inDays - amounts.length;
      
      for (int i = missingEntries; i > 0; i--)
      {
        amounts.add(0);
      }
    }

    return amounts;
  }

  static Map<String, dynamic> getExtremeDays(List<HistoryEntryGroup> history)
  {
    DateTime prevDate = DateTime(0);

    DateTime strongestDate = prevDate;
    int strongestXP = 0;

    DateTime weakestDate = prevDate;
    int weakestXP = 0;

    for (HistoryEntryGroup heg in history)
    {
      if (heg.total > strongestXP)
      {
        strongestXP = heg.total;
        strongestDate = prevDate;
      }

      if (heg.total < weakestXP || weakestXP == 0)
      {
        weakestXP = heg.total;
        weakestDate = prevDate;
      }

      prevDate = heg.getDate();
    }

    return {
      "strongestDate": strongestDate,
      "strongestXP": strongestXP,
      "weakestDate": weakestDate,
      "weakestXP": weakestXP,
    };
  }
}