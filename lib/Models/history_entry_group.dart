import 'package:intl/intl.dart';
import 'package:universal_io/io.dart';
import 'package:vextrack/Models/history_entry.dart';

class HistoryEntryGroup {
  DateTime date;
  List<HistoryEntry> entries;

  HistoryEntryGroup(this.date, this.entries);

  String getFormattedDate() {
    return DateFormat.yMd(Platform.localeName).format(date);
  }
}