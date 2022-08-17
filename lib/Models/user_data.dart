import 'package:cloud_firestore/cloud_firestore.dart';

class UserData
{
  String name;
  String imgURL;

  int streak;

  List<Map<String, dynamic>> contractIDs;
  List<Map<String, dynamic>> seasonIDs;

  UserData(this.name, this.imgURL, this.streak, this.contractIDs, this.seasonIDs);

  static UserData fromDoc(DocumentSnapshot doc)
  {
    return UserData(
      doc['username'] as String,
      doc['img'] as String,
      doc['streak'] as int,
      (doc['contractIDs'] as List<dynamic>).map((e) => e as Map<String, dynamic>).toList(),
      (doc['seasonIDs'] as List<dynamic>).map((e) => e as Map<String, dynamic>).toList(),
    );
  }
}