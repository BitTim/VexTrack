import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:vextrack/Models/settings_data.dart';

class UserData {
  String name;
  String imgURL;

  Map<String, ContractActivityMeta> contractIDs;
  Map<String, SeasonActivityMeta> seasonIDs;

  SettingsData? settings;

  UserData(
      this.name, this.imgURL, this.contractIDs, this.seasonIDs, this.settings);

  static UserData fromDoc(DocumentSnapshot doc) {
    return UserData(
      doc['username'] as String,
      doc['img'] as String,
      (doc['contractIDs'] as Map<String, dynamic>).map((key, value) => MapEntry(
          key,
          ContractActivityMeta(
              completed: value['completed'], paused: value['paused']))),
      (doc['seasonIDs'] as Map<String, dynamic>).map((key, value) =>
          MapEntry(key, SeasonActivityMeta(active: value['active']))),
      doc['settings'] == null
          ? null
          : SettingsData.fromMap(doc['settings'] as Map<String, dynamic>),
    );
  }

  Map<String, dynamic> toMap() {
    Map<String, dynamic> map = {};

    map['username'] = name;
    map['img'] = imgURL;
    map['contractIDs'] = contractIDs.map((key, value) =>
        MapEntry(key, {'completed': value.completed, 'paused': value.paused}));
    map['seasonIDs'] =
        seasonIDs.map((key, value) => MapEntry(key, {'active': value.active}));
    map['settings'] = settings!.toMap();

    return map;
  }
}

class SeasonActivityMeta {
  final bool active;
  SeasonActivityMeta({required this.active});
}

class ContractActivityMeta {
  final bool completed;
  final bool paused;
  ContractActivityMeta({required this.completed, required this.paused});
}
