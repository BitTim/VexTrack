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
    Map<String, dynamic> rawEntriesWithIDs = (doc['entries'] as Map<String, dynamic>);
    List<String> uuids = rawEntriesWithIDs.keys.toList();
    List<Map<String, dynamic>> rawEntries = rawEntriesWithIDs.values.map((e) => e as Map<String, dynamic>).toList();
    List<HistoryEntry> entries = [];

    for (int i = 0; i < rawEntries.length; i++)
    {
      entries.add(HistoryEntry.fromMap(rawEntries[i], uuids[i]));
    }

    entries.sort((a, b) => b.time.compareTo(a.time));

    return HistoryEntryGroup(
      doc.id,
      doc['day'] as int,
      doc['total'] as int,
      doc['date'] as Timestamp,
      entries,
    );
  }

  Map<String, dynamic> toMap()
  {
    Map<String, dynamic> map = {};

    Map<String, dynamic> rawEntries = {};
    for(HistoryEntry he in entries.reversed)
    {
      rawEntries[he.uuid] = he.toMap();
    }

    map['day'] = day;
    map['total'] = total;
    map['date'] = date;
    map['entries'] = rawEntries;

    return map;
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




  // --------------------------------
  // Setters
  // --------------------------------

  void addEntry(HistoryEntry he) {
    entries.add(he);
    entries.sort((a, b) => b.time.compareTo(a.time));
    total += he.xp;
  }

  void deleteEntry(HistoryEntry he) {
    if(!entries.contains(he)) return;
    entries.remove(he);
    total -= he.xp;
  }
}