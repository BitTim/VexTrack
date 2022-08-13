import 'package:cloud_firestore/cloud_firestore.dart';

class SeasonMeta
{
  String id;
  String name;
  List<List<String>> battlepass;
  List<List<String>> epilogue;
  int startDate;
  int endDate;
  String imgURL;

  SeasonMeta(this.id, this.name, this.battlepass, this.epilogue, this.startDate, this.endDate, this.imgURL);

  static SeasonMeta fromDoc(DocumentSnapshot doc, String id, List<List<String>> battlepass, List<List<String>> epilogue)
  {
    return SeasonMeta(
      id,
      doc['name'] as String,
      battlepass,
      epilogue,
      doc['start'] as int,
      doc['end'] as int,
      doc['img'] as String
    );
  }

  DateTime getDateTime(int timestamp) { return DateTime.fromMillisecondsSinceEpoch(timestamp * 1000).toLocal(); }
  DateTime getStartDate() { return getDateTime(startDate); }
  DateTime getEndDate() { return getDateTime(endDate); }
  Duration getDuration() { return getEndDate().difference(getStartDate()); }

  bool isActive() { return DateTime.now().compareTo(getEndDate()) < 0; }
}