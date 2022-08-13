import 'package:vextrack/Models/History/history_entry.dart';
import 'package:vextrack/Models/History/history_entry_group.dart';
import 'package:vextrack/Models/Seasons/season_meta.dart';

class HistoryCalc
{
  static List<HistoryEntryGroup> groupHistoryByDate(List<HistoryEntry> history) {
    List<HistoryEntryGroup> groupedHistory = [];

    for (HistoryEntry he in history)
    {
      if (groupedHistory.isEmpty)
      {
        groupedHistory.add(HistoryEntryGroup(he.getDate(), [he]));
      }
      else
      {
        if (groupedHistory.last.date == he.getDate()) {
          groupedHistory.last.entries.add(he);
        } else {
          groupedHistory.add(HistoryEntryGroup(he.getDate(), [he]));
        }
      }
    }

    return groupedHistory;
  }

  static List<int> getXPPerDay(List<HistoryEntry> history, SeasonMeta meta) {
    List<int> amounts = [0];
    DateTime prevDate = history[0].getDate();

    for (HistoryEntry he in history)
    {
      DateTime cDate = he.getDate();
      if(cDate == prevDate)
      {
        amounts.last += he.getXP();
      }
      else 
      {
        for (int i = 0; cDate.difference(prevDate).inDays - 1 > i; i++)
        {
          amounts.add(0);
        }

        amounts.add(he.getXP());
        prevDate = he.getDate();
      }
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

  static Map<String, dynamic> getExtremeDays(List<HistoryEntry> history)
  {
    DateTime prevDate = history[0].getDate();
    int dayTotal = 0;

    DateTime strongestDate = prevDate;
    int strongestXP = 0;

    DateTime weakestDate = prevDate;
    int weakestXP = 0;

    for (HistoryEntry he in history)
    {
      if (he.getDate() == prevDate)
      {
        dayTotal += he.xp;
      }
      else
      {
        if (dayTotal > strongestXP)
        {
          strongestXP = dayTotal;
          strongestDate = prevDate;
        }

        if (dayTotal < weakestXP || weakestXP == 0)
        {
          weakestXP = dayTotal;
          weakestDate = prevDate;
        }

        prevDate = he.getDate();
        dayTotal = he.xp;
      }
    }

    return {
      "strongestDate": strongestDate,
      "strongestXP": strongestXP,
      "weakestDate": weakestDate,
      "weakestXP": weakestXP,
    };
  }
}