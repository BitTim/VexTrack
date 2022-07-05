import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_storage/firebase_storage.dart';
import 'package:vextrack/Constants/references.dart';
import 'package:vextrack/Models/battlepass_params.dart';
import 'package:vextrack/Models/game_mode.dart';
import 'package:vextrack/Models/history_entry.dart';
import 'package:vextrack/Models/game_map.dart';
import 'package:vextrack/Models/season.dart';
import 'package:vextrack/Models/season_meta.dart';

class DataService
{
  static late BattlepassParams battlepassParams;
  static Map<String, GameMode> modes = {};
  static Map<String, GameMap> maps = {};
  static Map<String, SeasonMeta> seasonMetas = {};

  static void init() async
  {
    DataService.battlepassParams = await getBattlepassParams();
    DataService.modes = await getModes();
    DataService.maps = await getMaps();
    DataService.seasonMetas = await getSeasonMetas();
  }

  // ===============================
  //  Parameter data
  // ===============================

  static Future<BattlepassParams> getBattlepassParams() async
  {
    DocumentSnapshot doc = await parametersRef.doc('battlepass').get();
    return BattlepassParams.fromDoc(doc);
  }

  // ===============================
  //  User data
  // ===============================

  static Future<List<Season>> getAllSeasons(String uid) async {
    QuerySnapshot loadedSeasons = await usersRef.doc(uid)
      .collection("seasons")
      .orderBy("endDate", descending: true)
      .get();

    List<Season> seasons = loadedSeasons.docs.map((doc) => Season.fromDoc(doc)).toList();
    return seasons;
  }

  static Future<Season> getSeason(String uid, String uuid) async {
    DocumentSnapshot loadedSeason = await usersRef.doc(uid)
      .collection("seasons")
      .doc(uuid)
      .get();

    Season season = Season.fromDoc(loadedSeason);
    return season;
  }

  static Future<List<HistoryEntry>> getFullHistory(String uid) async {
    List<HistoryEntry> history = [];
    
    List<Season> seasons = await getAllSeasons(uid);
    for (Season season in seasons) {
      List<HistoryEntry> seasonHistory = await getHistory(uid, season.uuid);
      history.addAll(seasonHistory);
    }

    return history;
  }

  static Future<List<HistoryEntry>> getHistory(String uid, String uuid) async {
    QuerySnapshot loadedHistory = await usersRef.doc(uid)
      .collection("seasons")
      .doc(uuid)
      .collection("history")
      .orderBy("time", descending: true)
      .get();

    List<HistoryEntry> history = loadedHistory.docs.map((doc) => HistoryEntry.fromDoc(doc)).toList();
    return history;
  }

  // ===============================
  //  Season meta data
  // ===============================

  static Future<Map<String, SeasonMeta>> getSeasonMetas() async
  {
    QuerySnapshot loadedSeasonMetas = await seasonsRef.get();
    Map<String, SeasonMeta> seasonMetas = {};

    for(QueryDocumentSnapshot doc in loadedSeasonMetas.docs)
    {
      SeasonMeta seasonMeta = SeasonMeta.fromDoc(doc);
      seasonMetas[doc.id] = seasonMeta;
    }

    return seasonMetas;
  }

  static String getSeasonName(String id)
  {
    if(DataService.seasonMetas[id] == null) return "";
    return DataService.seasonMetas[id]!.name;
  }

  static String getSeasonFormattedStartDate(String id)
  {
    if(DataService.seasonMetas[id] == null) return "";
    return DataService.seasonMetas[id]!.getFormattedStartDate();
  }

  static String getSeasonFormattedEndDate(String id)
  {
    if(DataService.seasonMetas[id] == null) return "";
    return DataService.seasonMetas[id]!.getFormattedEndDate();
  }

  static Future<String> getSeasonImgUrl(String id) async
  {
    if(DataService.seasonMetas[id] == null) id = "none";
    String storageURL = DataService.seasonMetas[id]!.imgURL;
    String imgURL = await FirebaseStorage.instance.refFromURL(storageURL).getDownloadURL();
    return imgURL;
  }

  // ===============================
  //  Map data
  // ===============================

  static Future<Map<String, GameMap>> getMaps() async
  {
    QuerySnapshot loadedMaps = await mapsRef.get();
    Map<String, GameMap> maps = {};

    for (QueryDocumentSnapshot doc in loadedMaps.docs)
    {
      GameMap map = GameMap.fromDoc(doc);
      maps[doc.id] = map;
    }

    return maps;
  }

  static String getMapName(String id)
  {
    if(DataService.maps[id] == null) return "";
    return DataService.maps[id]!.name;
  }

  static Future<String> getMapImgUrl(String id) async
  {
    if(DataService.maps[id] == null) id = "none";
    String storageURL = DataService.maps[id]!.imgURL;
    String imgURL = await FirebaseStorage.instance.refFromURL(storageURL).getDownloadURL();
    return imgURL;
  }

  // ===============================
  //  Mode Data
  // ===============================

  static Future<Map<String, GameMode>> getModes() async
  {
    QuerySnapshot loadedModes = await modesRef.get();
    Map<String, GameMode> modes = {};

    for(QueryDocumentSnapshot doc in loadedModes.docs)
    {
      GameMode mode = GameMode.fromDoc(doc);
      modes[doc.id] = mode;
    }

    return modes;
  }

  static String getModeScoreFormat(String id)
  {
    if (DataService.modes[id] == null) return "";
    return DataService.modes[id]!.scoreFormat;
  }

  static int getModeScoreLimit(String id)
  {
    if (DataService.modes[id] == null) return -1;
    return DataService.modes[id]!.scoreLimit;
  }
}