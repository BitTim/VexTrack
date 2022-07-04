import 'package:vextrack/Models/history_entry_group.dart';

import '../Models/history_entry.dart';

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
}