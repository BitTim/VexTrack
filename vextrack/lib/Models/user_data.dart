import 'package:cloud_firestore/cloud_firestore.dart';

class UserData
{
  String name;
  String imgURL;

  int streak;

  Map<String, ContractActivityMeta> contractIDs;
  Map<String, SeasonActivityMeta> seasonIDs;

  UserData(this.name, this.imgURL, this.streak, this.contractIDs, this.seasonIDs);

  static UserData fromDoc(DocumentSnapshot doc)
  {
    return UserData(
      doc['username'] as String,
      doc['img'] as String,
      doc['streak'] as int,
      (doc['contractIDs'] as Map<String, dynamic>).map((key, value) => MapEntry(key, ContractActivityMeta(completed: value['completed'], paused: value['paused']))),
      (doc['seasonIDs'] as Map<String, dynamic>).map((key, value) => MapEntry(key, SeasonActivityMeta(active: value['active']))),
    );
  }
}

class SeasonActivityMeta
{
  final bool active;
  SeasonActivityMeta({required this.active});
}

class ContractActivityMeta
{
  final bool completed;
  final bool paused;
  ContractActivityMeta({required this.completed, required this.paused});
}