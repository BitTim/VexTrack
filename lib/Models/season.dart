import 'package:vextrack/Models/history_entry.dart';

class Season
{
  String uuid;
  String name;
  int activeLevel;
  int activeXP;
  String endDate;
  Set<HistoryEntry> history = {};

  Season(this.uuid, this.name, this.activeLevel, this.activeXP, this.endDate);

  static Season fromJSON(Map<String, dynamic> json)
  {
    return Season(
      json['uuid'] as String,
      json['name'] as String,
      json['activeLevel'] as int,
      json['activeXP'] as int,
      json['endDate'] as String
    );
  }
}