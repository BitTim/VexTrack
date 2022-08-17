import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Models/History/history_entry.dart';

class HistoryEntryGroup
{
  String id;
  int day;
  int total;
  Timestamp date;
  List<HistoryEntry> entries;

  HistoryEntryGroup(this.id, this.day, this.total, this.date, this.entries);

  static HistoryEntryGroup fromDoc(DocumentSnapshot doc)
  {
    List<Map<String, dynamic>> rawEntries = (doc['entries'] as List<dynamic>).map((e) => e as Map<String, dynamic>).toList();
    List<HistoryEntry> entries = rawEntries.map((e) => HistoryEntry.fromMap(e)).toList();

    return HistoryEntryGroup(
      doc.id,
      doc['day'] as int,
      doc['total'] as int,
      doc['date'] as Timestamp,
      entries.reversed.toList(),
    );
  }



  // --------------------------------
  // Getters
  // --------------------------------

  DateTime getDate() {
    return date.toDate();
  }



  // --------------------------------
  // Formatted getters
  // --------------------------------

  String getFormattedDate() {
    return Formatter.formatDate(date.toDate());
  }
}