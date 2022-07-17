import 'package:vextrack/Models/History/history_entry.dart';
import 'package:vextrack/Models/History/history_entry_group.dart';

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