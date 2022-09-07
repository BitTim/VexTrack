import 'package:cloud_firestore/cloud_firestore.dart';

class SeasonMeta
{
  String id;
  String name;
  List<List<String>> battlepass;
  List<List<String>> epilogue;
  Timestamp startDate;
  Timestamp endDate;

  SeasonMeta(this.id, this.name, this.battlepass, this.epilogue, this.startDate, this.endDate);

  static SeasonMeta fromDoc(DocumentSnapshot doc, String id, List<List<String>> battlepass, List<List<String>> epilogue)
  {
    return SeasonMeta(
      id,
      doc['name'] as String,
      battlepass,
      epilogue,
      doc['start'] as Timestamp,
      doc['end'] as Timestamp,
    );
  }

  DateTime getDateTime(Timestamp timestamp) { return timestamp.toDate().toLocal(); }
  DateTime getStartDate() { return getDateTime(startDate); }
  DateTime getEndDate() { return getDateTime(endDate); }
  Duration getDuration() { return getEndDate().difference(getStartDate()); }

  bool isActive() { return DateTime.now().compareTo(getEndDate()) < 0; }
}