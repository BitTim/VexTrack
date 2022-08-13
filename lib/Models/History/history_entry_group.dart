import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Models/History/history_entry.dart';

class HistoryEntryGroup {
  DateTime date;
  List<HistoryEntry> entries;

  HistoryEntryGroup(this.date, this.entries);




  // --------------------------------
  // Formatted getters
  // --------------------------------

  String getFormattedDate() {
    return Formatter.formatDate(date);
  }
}