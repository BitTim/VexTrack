import 'package:cloud_firestore/cloud_firestore.dart';

class Season
{
  String uuid;
  String name;
  int activeLevel;
  int activeXP;
  String endDate;

  Season(this.uuid, this.name, this.activeLevel, this.activeXP, this.endDate);

  static Season fromDoc(DocumentSnapshot doc)
  {
    return Season(
      doc['uuid'] as String,
      doc['name'] as String,
      doc['activeLevel'] as int,
      doc['activeXP'] as int,
      doc['endDate'] as String
    );
  }
}